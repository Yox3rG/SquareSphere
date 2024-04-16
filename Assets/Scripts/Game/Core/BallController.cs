using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public static BallController main { get; private set; } = null;

    public static event System.Action OnBallFired;
    public static event System.Action<int> OnNewBlocksBrokenInOneRoundHighScore;

    public GameObject ballPrefab;
    public GameObject particlePrefab;
    public GameObject PermanentPlaceHolder;

    public Color textColor = Color.white;

    public float BallScale 
    { 
        get { return scaleFactorOverride; }
        set
        {
            scaleFactorOverride = value;
            ballPrefab.transform.localScale = defaultScale * scaleFactorOverride;
            //ballPrefab.GetComponent<CircleCollider2D>().radius = defaultColliderRadius * scaleFactorOverride;
        }
    }
    private float scaleFactorOverride = 1;
    private Vector3 defaultScale = Vector3.one;
    private float defaultColliderRadius = .1f;

    private BallPainter ballPainter;

    private GameObject newPositionHolder;
    private GameObject currentPositionHolder;
    private SpriteRenderer newHolderSpriteRenderer;
    private SpriteRenderer currentHolderSpriteRenderer;
    // TODO: Only one bounce possible (aim line), and its not scalable as of yet.
    private AimLine aimLine;
    private AimLine aimLineBounce;
    private Text text;

    private int fallenBalls = 0;
    private int maxNumOfBalls = 25;
    private int bonusBalls = 0;
    private int newBonusBalls = 0;
    private int numOfBalls = 1;

    private bool areBallsShielded = false;

    private List<Ball> balls = new List<Ball>();

    private bool isRolling = false;
    private float nextTime;
    private const float cooldown = .05f;

    private long blocksBrokenAtStart = 0;
    private long blocksBrokenAtEnd = 0;
    private int blocksBrokenThisRound;

    private const float speedUpAmount = 4f;
    private const float speedUpCooldown = 6f;
    private float speedUpStart = 0f;
    private bool notSpedUp = true;

    private float textOffset = .2f;


    private float minTargetDegree = 10;
    // Automatically set in Start(), calculated from minTargetDegree.
    private float minTargetX;
    private float minTargetY;

    private float radiusOfBall;
    private float aimLineBounceLength = 2f;
    private int layerMaskOfWall = 1 << 12;

    private Vector3 target;
    private Vector3 targetDirection;
    private Vector3 newPosition;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    void Start()
    {
        isRolling = false;
        transform.position = new Vector3(transform.position.x, ScreenRelativeSizeCalculator.Instance.GetBallHeight(), transform.position.z);
        BallScale = ElementHandler.Instance.SquareScaleFactor;

        newPosition = transform.position;
        GamePlayMenu.main.SetGameState(isRolling);

        ballPainter = new BallPainter(ballPrefab);

        // Set up the placeholders.
        currentPositionHolder = new GameObject();
        currentPositionHolder.name = "Current Firing Position";
        currentPositionHolder.transform.position = transform.position;
        currentHolderSpriteRenderer = currentPositionHolder.AddComponent<SpriteRenderer>();
        currentHolderSpriteRenderer.sprite = BallTextureHolder.GetSelectedGameBallSprite();
        currentHolderSpriteRenderer.sortingLayerName = "Ball";

        newPositionHolder = new GameObject();
        newPositionHolder.name = "New Firing Position";
        newPositionHolder.transform.position = newPosition;
        newHolderSpriteRenderer = newPositionHolder.AddComponent<SpriteRenderer>();
        newHolderSpriteRenderer.sprite = currentHolderSpriteRenderer.sprite;
        newHolderSpriteRenderer.sortingLayerName = "Ball";

        PermanentPlaceHolder.GetComponent<SpriteRenderer>().sprite = currentHolderSpriteRenderer.sprite;
        PermanentPlaceHolder.SetActive(false);
        // Add the particle effect. (Comment this line to disable)
        //GameObject g = Instantiate(particlePrefab, newPositionHolder.transform, false);

        newPositionHolder.SetActive(false);

        if(MapDataBase.main != null)
        {
            maxNumOfBalls = MapDataBase.main.GetNumberOfBalls();
            if(maxNumOfBalls < 1)
            {
                maxNumOfBalls = 1;
            }
        }
        if(AttributeConverter.Instance != null)
        {
            Ball.MinSpeed = AttributeConverter.Instance.Get(Attribute.Type.BALL_SPEED);
        }

        GamePlayMenu.main.UpdateCosts();

        // Generate text for the number of balls.
        text = TextGenerator.main.Generate(transform.position + Vector3.up * textOffset, textColor);
        ResetText();

        minTargetY = Mathf.Sin(Mathf.Deg2Rad * minTargetDegree);
        minTargetX = Mathf.Cos(Mathf.Deg2Rad * minTargetDegree);


        radiusOfBall = ballPrefab.GetComponent<CircleCollider2D>().radius;
        aimLine = transform.Find("AimLine").GetComponent<AimLine>();
        aimLineBounce = transform.Find("AimLineBounce").GetComponent<AimLine>();
        aimLineBounce.SetDecreasingSizeActive(true);

        DisableLine();
    }

    private void OnEnable()
    {
        //ScreenRelativeSizeCalculator.Instance.OnCalculatingSquareScale += SetDifferentScale;
    }

    private void OnDisable()
    {
        //ScreenRelativeSizeCalculator.Instance.OnCalculatingSquareScale -= SetDifferentScale;
    }

    void Update()
    {
        if (isRolling)
        {
            nextTime -= Time.deltaTime;
            /*
            if(rollSpeedUpStart < Time.time && speedUpCountLeft > 0)
            {
                SpeedUpTimeAndSetThreshold(speedUpThresholdDivider);
            }
            */
            if(speedUpStart < Time.time && notSpedUp)
            {
                SpeedUp();
            }

            if(nextTime < 0 && numOfBalls > 0)
            {
                SendBallTo(targetDirection);
                numOfBalls--;
                nextTime = cooldown;

                if(numOfBalls <= 0)
                {
                    currentPositionHolder.SetActive(false);
                    //currentHolderSpriteRenderer.enabled = false;
                    areBallsShielded = false;
                    //transform.position = newPosition;
                }
            }

            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    StopRolling();
            //}
        }
        else
        {
            if (IsTargeting())
            {
                if(Input.touchCount == 1)
                {
                    HandleTouchInteraction();
                }
                else
                {
                    HandleMouseInteraction();
                }
            }
            else
            {
                DisableLine();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            Time.timeScale = 0;
        }
    }



    public void FirstBallFell(Vector3 position)
    {
        newPosition = new Vector3(position.x, transform.position.y);

        if(numOfBalls <= 0)
        {
            transform.position = newPosition;
        }
        SetNewHolderActive(true);
    }

    public Vector3 GetNewLocation()
    {
        return newPosition;
    }

    public void BallIsGone()
    {
        if (++fallenBalls == maxNumOfBalls + bonusBalls)
        {
            StopRolling();
        }
    }


    private void ResetText()
    {
        text.text = maxNumOfBalls + " + " + bonusBalls;
    }

    private void SetText(Vector3 position)
    {
        TextGenerator.main.Move(text.gameObject, position);
        ResetText();
    }

    private void SetNewHolderActive(bool value)
    {
        newPositionHolder.transform.position = newPosition;
        newPositionHolder.SetActive(value);
    }

    private void SetCurrentHolderActive(bool value, Vector3 position)
    {
        currentPositionHolder.transform.position = position;
        currentPositionHolder.SetActive(value);
    }

    /*
    private void SetSpeedUpSettingsToDefault()
    {
        currentSpeedUpThreshold = maxSpeedUpThreshold;
        speedUpCountLeft = maxSpeedUpCount;

        rollSpeedUpStart = Time.time + currentSpeedUpThreshold;
    }
    
    private void SpeedUpTimeAndSetThreshold(float divider)
    {
        Time.timeScale *= speedUpAmount;

        currentSpeedUpThreshold /= divider;
        speedUpCountLeft--;

        rollSpeedUpStart = Time.time + currentSpeedUpThreshold;
    }
    */
    private void StartSpeedUpCountDown()
    {
        speedUpStart = Time.time + speedUpCooldown;
        notSpedUp = true;
    }

    private void SpeedUp()
    {
        Time.timeScale *= speedUpAmount;
        notSpedUp = false;

        GamePlayMenu.main.SetSpeedUp(true);
    }

    private void StartRolling(Vector3 screenPosition)
    {
        target = Camera.main.ScreenToWorldPoint(screenPosition);
        target.z = 0;
        targetDirection = CalculateRealVelocity(target);
        
        isRolling = true;
        nextTime = 0;
        numOfBalls = maxNumOfBalls + bonusBalls;
        fallenBalls = 0;

        //SetSpeedUpSettingsToDefault();
        StartSpeedUpCountDown();

        blocksBrokenAtStart = ProfileDataBase.main?.GetBlocksBroken() ?? 696969;

        text.enabled = false;

        BallBasket.main.GameIsRolling();
        GamePlayMenu.main.SetGameState(isRolling);
        SquareDataBase.Instance.HideWarning();
    }

    public void StopRolling()
    {
        if (isRolling)
        {
            isRolling = false;
            Time.timeScale = 1;

            SquareDataBase.Instance.NextRound();
            BallBasket.main.GameStoppedRolling();
            GamePlayMenu.main.SetGameState(isRolling);

            transform.position = newPosition;
            currentHolderSpriteRenderer.enabled = true;
            SetNewHolderActive(false);
            SetCurrentHolderActive(true, newPosition);
            CollectBalls();

            bonusBalls += newBonusBalls;
            newBonusBalls = 0;

            ballPainter.Reset();

            GamePlayMenu.main.SetSpeedUp(false);
            GamePlayMenu.main.EnableAllSAButtons();

            if(ProfileDataBase.main != null)
            {
                blocksBrokenAtEnd = ProfileDataBase.main.GetBlocksBroken();
                blocksBrokenThisRound = (int)(blocksBrokenAtEnd - blocksBrokenAtStart);

                ProfileDataBase.main.AppendLineToChainAnalysis(blocksBrokenThisRound);

                if(blocksBrokenThisRound > ProfileDataBase.main.GetBlocksBrokenOneRound())
                {
                    OnNewBlocksBrokenInOneRoundHighScore?.Invoke(blocksBrokenThisRound);
                }
            }

            SetText(newPosition + Vector3.up * textOffset);
            text.enabled = true;
        }
    }

    #region Targeting
    private void HandleTouchInteraction()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                target = Camera.main.ScreenToWorldPoint(touch.position);
                target.z = 0;
                DrawLineTo(CalculateRealVelocity(target));
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                DisableLine();
                StartRolling(touch.position);
            }
        }
    }

    private void HandleMouseInteraction()
    {
        if (Input.GetMouseButton(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            DrawLineTo(CalculateRealVelocity(target));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DisableLine();
            StartRolling(Input.mousePosition);
        }
    }

    private void DisableLine()
    {
        aimLine.Hide();
        aimLineBounce.Hide();
    }

    private void DrawLineTo(Vector3 targetDirection)
    {
        Vector2 rayWallHitPosition = FindTargetWallHit(targetDirection, out Vector2 normal);


        float tanValue = Mathf.Tan(Vector2.Angle(-normal, targetDirection) * Mathf.Deg2Rad);
        float lengthDifference = tanValue * radiusOfBall;
        Vector2 ballCenterOnHit;

        if (normal.x != 0)
        {
            // If the ray hit one of the sides...
            ballCenterOnHit = new Vector2(rayWallHitPosition.x - Mathf.Sign(rayWallHitPosition.x) * radiusOfBall, rayWallHitPosition.y - lengthDifference);
        }
        else
        {
            // ... or the top.
            ballCenterOnHit = new Vector2(rayWallHitPosition.x - Mathf.Sign(rayWallHitPosition.x) * lengthDifference, rayWallHitPosition.y - radiusOfBall);
        }


        float leftOver = aimLine.Draw(newPosition, ballCenterOnHit);
        aimLineBounce.Draw(ballCenterOnHit, ballCenterOnHit + Vector2.Reflect(targetDirection, normal) * aimLineBounceLength, leftOver);
    }

    private Vector2 FindTargetWallHit(Vector3 direction, out Vector2 normal)
    {
        RaycastHit2D hit;
        if(hit = Physics2D.Raycast(newPosition, direction, 100f, layerMaskOfWall))
        {
            Debug.DrawLine(newPosition, hit.point, Color.blue);
            normal = hit.normal;
            return hit.point;
        }

        // Default return values.
        normal = Vector2.zero;
        return Vector2.zero;
    }

    // HACK this can be correct, but i would not be suprised if it is not.
    private bool IsTargeting()
    {
        bool value = false;

        bool isOverUI = false;
        if(Input.touchCount == 1)
        {
            isOverUI = IsTouchOverUI(Input.GetTouch(0));
        }

        if(!EventSystem.current.IsPointerOverGameObject(-1) && !isOverUI)
        {
            value = true;
        }
        /*
        if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            value = true;
        }
        */
        if ((EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 11))
        {
            value = true;
            Debug.Log("second");
        }
        return value;
    }

    private bool IsTouchOverUI(Touch touch)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touch.position;
        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Count != 0;
    }
    #endregion

    #region SpecialAbility
    public void SpecialAbilityGoldCannon(int bonusBallCount)
    {
        bonusBalls += bonusBallCount;
        newBonusBalls -= bonusBallCount;

        ballPainter.ShootGoldAfter(bonusBallCount);

        ResetText();
    }

    public void SpeciaAbilityShieldForBalls()
    {
        areBallsShielded = true;
    }
    #endregion

    #region PowerUpRelated
    public void BonusBallPowerUp()
    {
        newBonusBalls++;
    }
    #endregion

    private Vector3 CalculateRealVelocity(Vector3 target)
    {
        Vector3 velocity = (target - transform.position).normalized;

        if (velocity.y < minTargetY)
        {
            velocity.x = Mathf.Sign(velocity.x) * minTargetX;
            velocity.y = minTargetY;
        }

        velocity.z = 0;

        return velocity.normalized;
    }

    // The target and the transform.position should have their z = 0;
    private void SendBallTo(Vector3 direction)
    {
        OnBallFired?.Invoke();
        Ball b = CreateBall();
        b.SetVelocity(direction);
    }

    private Ball CreateBall()
    {
        GameObject g = Instantiate(ballPainter.GetCurrentPrefab(), transform.position, Quaternion.identity);
#if UNITY_EDITOR
        g.name = "Ball" + numOfBalls;
#endif
        Ball ball = g.GetComponent<Ball>();
        g.SetActive(true);
        ball.IsShielded = areBallsShielded;
        balls.Add(ball);

        ballPainter.BallShot();

        return ball;
    }
    
    private void CollectBalls()
    {
        foreach(Ball b in balls)
        {
            if(b != null)
            {
                b.Return();
            }
        }
        balls.Clear();
    }

    public int GetCurrentAmountOfBalls()
    {
        return maxNumOfBalls + bonusBalls + newBonusBalls;
    }

    public int GetNotBonusNumberOfBalls()
    {
        return maxNumOfBalls;
    }
}
