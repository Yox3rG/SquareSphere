using System;

[Serializable]
public class UnlockBall
{
    // Identifies Spritesheet.
    public int generation;
    // Random ball in generation.
    public bool isRandom;
    // Index of ball in generation. Not used if random.
    public int index;

    public UnlockBall() : this(0, 0, isRandom: false) {}

    public UnlockBall(int gen, int index) : this(gen, index, isRandom: false) {}

    public UnlockBall(int gen, int index, bool isRandom)
    {
        generation = gen;
        this.index = index;
        this.isRandom = isRandom;
    }
}