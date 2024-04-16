using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(CurrencyIconHolder))]
public class ProfileDataBase : MonoBehaviour
{
    public static ProfileDataBase main = null;

    public static readonly DateTime officialLaunchDay = DateTime.Parse("2020-06-27T14:17:33.941979+00:00");

    private DataCollection data;
    private StatisticRegister sr;

    private readonly bool isAnalysing = true;
    public const string chainAnalysisFileName = "chainAnalysis";
    public const string logDailiesFileName = "log_dailies";

    private readonly int[] goldGainedPerStar = { 5, 10, 15 };
    private readonly int[] xpGainedPerStar = { 2, 3, 4 };

    private readonly int[] lvlRequirements =
        { 0, 10, 14, 22, 34, 46, 60, 76, 92, 112, 132, 154, 178, 202, 230, 258, 288, 320, 352, 388, 424,
        462, 502, 542, 586, 630, 676, 724, 772, 828, 888, 952, 1020, 1092, 1168, 1248, 1332, 1420, 1512, 1608, 1708, 696969 }; // The last one would be for lvl 41.
    private readonly int maxLvl = 40;

    private float timeSavedInCurrentSession;

    // HACK This file layout is different in the way it declares variables.
    // More specific variables are defined right before the section that uses them.
    // Enums are exceptions.
    public enum PaymentResult
    {
        SUCCESS,
        WATCHING_AD,
        NOT_ENOUGH_CURRENCY,
        NO_SUCH_ITEM,
        ALREADY_PURCHASED,
        UNSPECIFIED_ERROR
    }

    void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        data = new DataCollection();
        string dataFolder = Path.Combine(Application.persistentDataPath, data.GetFileLocation());
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        if (!data.LoadData())
        {
            SetDefaultData();
        }

        data.shop.UpdateCountOfUnlockedBalls();
    }

    void Start()
    {
        sr = new StatisticRegister(this);
        timeSavedInCurrentSession = 0;
        //DebugFunction();
    }

    private void OnDestroy()
    {
        if (sr != null)
        {
            sr.UnRegisterActions();
        }
    }

    private void Update()
    {
    }

    // HACK debug.
    #region DEBUG
#if UNITY_EDITOR
    private void LvlDebug(int xpGained)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GainXp(xpGained);

            Debug.Log("You're lvl " + data.profile.level + ", and have " + data.profile.experienceOnCurrentLevel + " xp on this level.");
            Debug.Log("You need " + NextLvlRequirement(data.profile.level) + " xp for next level, and gained " + data.stats.experienceGained + " xp overall.");

            Debug.Log(ValidLvlCheck() ? "You have a valid lvl." : "Your lvl is invalid.");
        }
    }

    private void DebugFunction()
    {
        /*
        for (int i = 0; i < 20; i++)
        {
            Debug.Log("NextLvlReq " + i + " : " + NextLvlRequirement(i));
        }
        */
        for (int i = 0; i < 1000; i++)
        {
        }
    }

    private float TestTimeNormal(int i)
    {
        float start = Time.realtimeSinceStartup;

        XpNeededForLvl(i);

        float end = Time.realtimeSinceStartup;

        return end - start;
    }
#endif
    #endregion


    #region CurrencySection
    public void GainCurrency(Currency currency)
    {
#if UNITY_EDITOR
        Debug.Log("Gained " + currency.ToString());
#endif
        switch (currency.type)
        {
            case Currency.Type.GOLD:
                data.profile.gold += currency.amount;
                break;
            case Currency.Type.DIAMOND_APPLE:
                data.profile.diamondApples += currency.amount;
                break;
            case Currency.Type.AD_VIDEO:
            default:
                return;
        }
    }

    private int StarsToGold(int oldStarCount, int newStarCount)
    {
        int amount = 0;
        for (int i = oldStarCount; i < newStarCount; i++)
        {
            amount += goldGainedPerStar[i];
        }
        return amount;
    }

    private bool SpendCurrency(Currency cost)
    {
        ref int currency = ref data.profile.gold;
        switch(cost.type)
        {
            case Currency.Type.GOLD:
                // Already assigned at declaration.
                break;
            case Currency.Type.DIAMOND_APPLE:
                currency = ref data.profile.diamondApples;
                break;
            case Currency.Type.AD_VIDEO:
                AdManager.Instance.ShowAd(AdManager.AdType.REWARDED);
                return true;
            default:
                return false;
        }

        int tempCurrency = currency - cost.amount;
        if (tempCurrency >= 0)
        {
            currency = tempCurrency;
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetCurrentGold()
    {
        return data.profile.gold;
    }
    public int GetCurrentDiamondApples()
    {
        return data.profile.diamondApples;
    }

    // Rewarded ad is not really fitting in the currency category,
    // but it is part of it atm.
    public PaymentResult BuyActionForRewardedVideo(Action action)
    {
        Currency cost = new Currency(Currency.Type.AD_VIDEO, 1);
        AdManager.Instance.SetRewardAction(action);

        if (!SpendCurrency(cost))
        {
            return PaymentResult.UNSPECIFIED_ERROR;
        }
        else
        {
            return PaymentResult.WATCHING_AD;
        }

        //return PaymentResult.UNSPECIFIED_ERROR;
    }

    public PaymentResult BuySpecialAbility(ISpecialAbility ability)
    {
        Currency cost = ability.GetCost();

        // TODO: Get a real gold amount.
        if (!SpendCurrency(cost))
        {
            return PaymentResult.NOT_ENOUGH_CURRENCY;
        }
        else
        {
            return PaymentResult.SUCCESS;
        }

        //return PaymentResult.UNSPECIFIED_ERROR;
    }

    public PaymentResult BuyBall(int index)
    {
        if (index >= data.shop.UnlockableBallListCount() || index < 0)
        {
            return PaymentResult.NO_SUCH_ITEM;
        }
        else if (IsBallUnLocked(index))
        {
            return PaymentResult.ALREADY_PURCHASED;
        }

        Currency cost = data.shop.GetBallCost(index);

        if(cost.type == Currency.Type.AD_VIDEO)
        {
            return BuyActionForRewardedVideo(delegate { UnlockBall(new UnlockBall(0, index)); });
        }
        else if (!SpendCurrency(cost))
        {
            return PaymentResult.NOT_ENOUGH_CURRENCY;
        }
        else
        {
            data.shop.ballsUnlocked[index] = true;
            data.shop.selectedBall = index;

            SaveData();
            return PaymentResult.SUCCESS;
        }

        //return PaymentResult.UNSPECIFIED_ERROR;
    }

    public bool UnlockBall(UnlockBall ball, bool equipNew = true)
    {
        int index = ball.index;
        if (ball.isRandom)
        {
            if (IsBallSheetGenerationFinished(ball.generation, out List<int> lockedBalls))
                return false;

            index = lockedBalls[UnityEngine.Random.Range(0, lockedBalls.Count)];
        }
        else
        {
            if (index >= data.shop.UnlockableBallListCount() || index < 0)
            {
                return false;
            }
            else if (IsBallUnLocked(index))
            {
                return false;
            }
        }
#if UNITY_EDITOR
        Debug.Log("Unlocked ball number: " + index.ToString());
#endif
        data.shop.ballsUnlocked[index] = true;
        data.shop.selectedBall = index;

        SaveData();
        return true;
        //return PaymentResult.UNSPECIFIED_ERROR;
    }
#endregion


#region LevelCompletition
    public void CompleteLevel(int level, byte starScore)
    {
        if(starScore >= 0 && starScore < 4)
        {
            // Count here represents the next index in the list.
            int nextIncompleteLevel = data.profile.starsOnMaps.Count;

            if (nextIncompleteLevel == level)
            {
                GainXp(StarsToXp(0, starScore));
                GainCurrency(new Currency(Currency.Type.GOLD, StarsToGold(0, starScore)));
                //Debug.Log(StarsToXp(0, starScore));

                data.profile.starsOnMaps.Add(starScore);
                data.profile.stars += starScore;
            }
            else if(nextIncompleteLevel > level)
            {
                int differenceInStars = starScore - data.profile.starsOnMaps[level];
                if (0 < differenceInStars)
                {
                    GainXp(StarsToXp(data.profile.starsOnMaps[level], starScore));
                    GainCurrency(new Currency(Currency.Type.GOLD, StarsToGold(data.profile.starsOnMaps[level], starScore)));
                    //Debug.Log(StarsToXp(data.profile.starsOnMaps[level], starScore));

                    data.profile.starsOnMaps[level] = starScore;
                    data.profile.stars += differenceInStars;
                }
            }
            else
            {
                Debug.Log("Invalid level completed. (level number too high)");
            }
        }
    }

    private int StarsToXp(int oldStarCount, int newStarCount)
    {
        int amount = 0;
        for(int i = oldStarCount; i < newStarCount; i++)
        {
            amount += xpGainedPerStar[i];
        }
        return amount;
    }

    public bool IsLevelPlayable(int level)
    {
        if(level <= data.profile.starsOnMaps.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetLastPlayableMap()
    {
        return data.profile.starsOnMaps.Count;
    }

    public byte GetStarsOnMap(int level)
    {
        return data.profile.starsOnMaps[level];
    }

    public long GetCurrentStars()
    {
        return data.profile.stars;
    }
#endregion


#region Shop
    public Currency GetBallCost(int index)
    {
        return data.shop.GetBallCost(index);
    }

    public void SelectBall(int index)
    {
        data.shop.selectedBall = index;
    }

    public int GetSelectedBallIndex()
    {
        return data.shop.selectedBall;
    }

    public bool IsBallUnLocked(int index)
    {
        return data.shop.IsBallUnLocked(index);
    }

    // TODO: Ballsheet generation
    private bool IsBallSheetGenerationFinished(int generation, out List<int> lockedBalls)
    {
        lockedBalls = data.shop.GetLockedBallIndexes();
        return lockedBalls.Count == 0;
    }
#endregion

#region ExperienceSection
    public void GainXp(int amount)
    {
        data.profile.experienceOnCurrentLevel += amount;
        data.stats.experienceGained += amount;

        while(data.profile.experienceOnCurrentLevel >= NextLvlRequirement(data.profile.level) && data.profile.level < maxLvl)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        data.profile.experienceOnCurrentLevel -= NextLvlRequirement(data.profile.level);
        data.profile.level++;
    }

    private long NextLvlRequirement(int currentLvl)
    {
        return System.Convert.ToInt64(lvlRequirements[currentLvl + 1]);
    }

    private bool ValidLvlCheck()
    {
        return XpNeededForLvl(data.profile.level) + data.profile.experienceOnCurrentLevel == data.stats.experienceGained;
    }

    private long XpNeededForLvl(int lvl)
    {
        long amount = 0;
        for (int i = 0; i <= lvl; i++)
        {
            amount += lvlRequirements[i];
            Debug.Log(amount);
        }

        return amount;
    }

    public long GetExperienceRequiredOnCurrentLevel()
    {
        return NextLvlRequirement(GetCurrentLevel());
    }

    public long GetExperienceOnCurrentLevel()
    {
        return data.profile.experienceOnCurrentLevel;
    }

    public float GetExperiencePercentageOnCurrentLevel()
    {
        return (float)data.profile.experienceOnCurrentLevel / NextLvlRequirement(GetCurrentLevel());
    }

    public int GetCurrentLevel()
    {
        return data.profile.level;
    }
#endregion

#region StatSection
    public string GetStatisticString(out int count)
    {
        CountingStringBuilder stringBuilder = new CountingStringBuilder();
        stringBuilder
            .AppendLine("Levels failed: " + data.stats.deathCount)
            .AppendLine("Levels completed: " + (GetLastPlayableMap() - 1))
            .AppendLine("Levels started: " + data.stats.levelsStarted)
            .AppendLine("Levels quit: " + data.stats.levelsQuit)
            .AppendLine("Balls fired: " + data.stats.ballsFired)
            .AppendLine("Blocks broken: " + data.stats.blocksBroken)
            .AppendLine("... in one round: " + data.stats.blocksBrokenOneRound)
            .AppendLine("Damage dealt: " + Math.Floor(data.stats.damageDealt))
            .AppendLine("Highest score: " + data.stats.highestScore)
            .AppendLine("Balls unlocked: " + data.shop.UnlockedBallCount())
            .AppendLine("")
            .AppendLine("Bosses killed: " + data.stats.bossKilled)
            .AppendLine("CubeKing killed: " + data.stats.bossCubeKingKilled)
            .AppendLine("Cactus killed: " + data.stats.bossCactusKilled)
            .AppendLine("Squid killed: " + data.stats.bossSquidKilled);
        // This is the static variant.
        //stringBuilder.AppendLine("Game open for: " + data.stats.timeSpent.ToString("F1") + " seconds");

        count = stringBuilder.Count;

        return stringBuilder.ToString();
    }

    public void StatIncreaseDeath()
    {
        data.stats.deathCount++;
    }

    public void StatIncreaseLevelsStarted()
    {
        data.stats.levelsStarted++;
    }

    public void StatIncreaseLevelsQuit()
    {
        data.stats.levelsQuit++;
    }

    public void StatIncreaseBallsFired()
    {
        data.stats.ballsFired++;
    }

    public void StatIncreaseBlocksBroken()
    {
        data.stats.blocksBroken++;
    }

    public void StatSetBlocksBrokenOneRound(int value)
    {
        data.stats.blocksBrokenOneRound = value;
    }

    public void StatIncreaseDamageDealt(float damage)
    {
        data.stats.damageDealt += damage;
    }

    public void StatSetHighestScore(int value)
    {
        data.stats.highestScore = value;
    }

    public void StatIncreaseTimeSpent()
    {
        data.stats.timeSpent += Time.time - timeSavedInCurrentSession;
        timeSavedInCurrentSession = Time.time;
    }

    // Boss related
    public void StatIncreaseBossCubeKilled()
    {
        data.stats.bossCubeKingKilled++;
        StatIncreaseBossKilled();
    }

    public void StatIncreaseBossCactusKilled()
    {
        data.stats.bossCactusKilled++;
        StatIncreaseBossKilled();
    }

    public void StatIncreaseBossSquidKilled()
    {
        data.stats.bossSquidKilled++;
        StatIncreaseBossKilled();
    }

    // Called automatically when a specific type is killed.
    private void StatIncreaseBossKilled()
    {
        data.stats.bossKilled++;
    }

    // Getters ---------------------
    public long GetBlocksBroken()
    {
        return data.stats.blocksBroken;
    }

    public int GetBlocksBrokenOneRound()
    {
        return data.stats.blocksBrokenOneRound;
    }

    public int GetHighestScore()
    {
        return data.stats.highestScore;
    }

    public double GetTimeSpent()
    {
        return data.stats.timeSpent;
    }
#endregion

#region Settings
    public void ChangeDefaultMasterVolume(float value)
    {
        data.settings.masterVolume = value;
    }

    public void ChangeDefaultMusicVolume(float value)
    {
        data.settings.musicVolume = value;
    }

    public void ChangeDefaultSFXVolume(float value)
    {
        data.settings.sfxVolume = value;
    }

    public Dictionary<string, float> GetMusicVolumes()
    {
        Dictionary<string, float> d = new Dictionary<string, float>();
        d.Add("masterVol", data.settings.masterVolume);
        d.Add("musicVol", data.settings.musicVolume);
        d.Add("sfxVol", data.settings.sfxVolume);

        return d;
    }

    public bool GetIsChallengeMode()
    {
        return data.settings.isChallengeMode;
    }
    #endregion

#region DailyReward
    public DateTime GetLastLogin()
    {
        return data.dailyReward.LastRewardDateTime;
    }

    public int GetDailyDayCount()
    {
        return data.dailyReward.allDayCount;
    }

    public void DailyRewardClaimed(DateTime claimedAt, RewardPackage package)
    {
        AppendLineToDailyLog(claimedAt, package);

        data.dailyReward.allDayCount++;
        data.dailyReward.LastRewardDateTime = claimedAt;
    }
    #endregion

    public void ChangeName(string name)
    {
        data.profile.name = name;
    }

    public string GetName()
    {
        return data.profile.name;
    }

    private void SetDefaultData()
    {
        data.profile = new ProfileData();
        data.stats = new StatisticsData();
        data.shop = new ShopData();

        data.shop.ballsUnlocked = new List<bool>();
        data.shop.ballsUnlocked.Add(true);
        for (int i = 1; i < BallTextureHolder.GetBallSpriteCount(); i++)
        {
            data.shop.ballsUnlocked.Add(false);
        }
    }

    public bool CreateHeaderToChainAnalysis()
    {
        if (isAnalysing && !SaveLoadHandlerJSON<int>.FileExists(chainAnalysisFileName))
        {
            SaveLoadHandlerJSON<string>.AppendLine(
                "Level #,Round #,count_balls,count_chain,count_visible",
                chainAnalysisFileName);
        }
        return false;
    }

    public bool AppendLineToChainAnalysis(int chainCount)
    {
        if (isAnalysing)
        {
            int level_num, round_num, count_balls, count_chain, count_visible;
            level_num = round_num = count_balls = count_chain = count_visible = 0;

            level_num = Menu.main.GetCurrentLvl();
            round_num = SquareDataBase.Instance?.GetCurrentRoundNumber() ?? 696969;
            count_balls = BallController.main?.GetCurrentAmountOfBalls() ?? 696969;
            count_chain = chainCount;
            count_visible = SquareDataBase.Instance?.GetVisibleDestroyableCount() ?? 696969;

            char separator = ',';

            System.Text.StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(level_num).Append(separator);
            stringBuilder.Append(round_num).Append(separator);
            stringBuilder.Append(count_balls).Append(separator);
            stringBuilder.Append(count_chain).Append(separator);
            stringBuilder.Append(count_visible);


            return SaveLoadHandlerJSON<string>.AppendLine(
                stringBuilder.ToString(),
                chainAnalysisFileName);
        }
        return false;
    }

    public bool AppendLineToDailyLog(DateTime date, RewardPackage package)
    {
        char separator = ',';

        System.Text.StringBuilder stringBuilder = new StringBuilder();
        stringBuilder
            .Append(date.ToString()).Append(separator)
            .Append(data.dailyReward.allDayCount).Append(separator)
            .Append(package.ToString());


        return SaveLoadHandlerJSON<string>.AppendLine(
            stringBuilder.ToString(),
            logDailiesFileName);
    }

    public bool SaveData()
    {
        return data.SaveData();
    }
}
