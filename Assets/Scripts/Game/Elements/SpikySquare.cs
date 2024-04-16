


public class SpikySquare : Destroyable
{

    public override void SetColor(byte colorIndex)
    {
        base.SetColor(colorIndex);
    }

    public override void CollidedWith(Ball ball)
    {
        base.CollidedWith(ball);
        BallController.main.BallIsGone();
        ball.Break();
    }
}
