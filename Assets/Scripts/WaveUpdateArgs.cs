using Unity.VisualScripting;

[Inspectable]
public class WaveUpdateArgs
{
    [Inspectable] public int wave;
    [Inspectable] public int enemyCount;

    public WaveUpdateArgs(int wave, int enemyCount)
    {
        this.wave = wave;
        this.enemyCount = enemyCount;
    }
}