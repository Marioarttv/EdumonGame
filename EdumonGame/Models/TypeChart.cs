namespace EdumonGame.Models
{
    public static class TypeChart
    {
        static float[][] chart =
    {
        //                   NOR    FIG   FLY   POI   GRO   ROC   BUG   GHO   STE   FIR   WAT   GRA   ELE   PSY   ICE   DRA   DAR   FAI
        /*NOR*/ new float[] { 1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   0f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f },
        /*FIG*/ new float[] { 2f,   1f, 0.5f, 0.5f,   1f,   2f, 0.5f,   0f,   2f,   1f,   1f,   1f,   1f, 0.5f,   2f,   1f,   2f, 0.5f },
        /*FLY*/ new float[] { 1f,   2f,   1f,   1f,   1f, 0.5f,   2f,   1f, 0.5f,   1f,   1f,   2f, 0.5f,   1f,   1f,   1f,   1f,   1f },
        /*POI*/ new float[] { 1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f, 0.5f,   0f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f },
        /*GRO*/ new float[] { 1f,   1f,   0f,   2f,   1f,   2f, 0.5f,   1f,   2f,   2f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   1f },
        /*ROC*/ new float[] { 1f, 0.5f,   2f,   1f, 0.5f,   1f,   2f,   1f, 0.5f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f },
        /*BUG*/ new float[] { 1f, 0.5f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f,   2f,   1f,   2f,   1f,   1f,   2f, 0.5f },
        /*GHO*/ new float[] { 0f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f,   1f },
        /*STE*/ new float[] { 1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f, 0.5f, 0.5f,   1f, 0.5f,   1f,   2f,   1f,   1f,   2f },
        /*FIR*/ new float[] { 1f,   1f,   1f,   1f,   1f, 0.5f,   2f,   1f,   2f, 0.5f, 0.5f,   2f,   1f,   1f,   2f, 0.5f,   1f,   1f },
        /*WAT*/ new float[] { 1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f,   1f, 0.5f,   1f,   1f },
        /*GRA*/ new float[] { 1f,   1f, 0.5f, 0.5f,   2f,   2f, 0.5f,   1f, 0.5f, 0.5f,   2f, 0.5f,   1f,   1f,   1f, 0.5f,   1f,   1f },
        /*ELE*/ new float[] { 1f,   1f,   2f,   1f,   0f,   1f,   1f,   1f,   1f,   1f,   2f, 0.5f, 0.5f,   1f,   1f, 0.5f,   1f,   1f },
        /*PSY*/ new float[] { 1f,   2f,   1f,   2f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   0f,   1f },
        /*ICE*/ new float[] { 1f,   1f,   2f,   1f,   2f,   1f,   1f,   1f, 0.5f, 0.5f, 0.5f,   2f,   1f,   1f, 0.5f,   2f,   1f,   1f },
        /*DRA*/ new float[] { 1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0f },
        /*DAR*/ new float[] { 1f, 0.5f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   1f, 0.5f, 0.5f },
        /*FAI*/ new float[] { 1f,   2f,   1f, 0.5f,   1f,   1f,   1f,   1f, 0.5f, 0.5f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f },
    };

        public static float GetEffectiveness(EdumonType attackType, EdumonType defendType)
        {
            if (attackType == EdumonType.None || defendType == EdumonType.None)
                return 1f;

            int row = (int)attackType - 1;
            int col = (int)defendType - 1;

            return chart[row][col];
        }
    }
}