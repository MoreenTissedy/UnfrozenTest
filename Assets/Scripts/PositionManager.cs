using UnityEngine;

namespace UnfrozenTest
{
    /// <summary>
    /// This class calculates and provides character positions on the screen.
    /// </summary>
    public class PositionManager
    {
        private static float YPosition = 0, ZPosition = 0, XCenter = 0;
        public static readonly int squadLength = 4;
        
        //numeration starts from the center, allies to the left, enemies to the right
        private Vector3[] allies;
        public Vector3[] Allies => allies;

        private Vector3[] enemies;
        public Vector3[] Enemies
        {
            get => enemies;
            set => enemies = value;
        }

        public PositionManager(float centralGap, float gap)
        {
            //cache character positions
            allies = new Vector3[squadLength];
            Enemies = new Vector3[squadLength];
            Vector3 center = new Vector3(XCenter, YPosition, ZPosition);
            for (int i = 0; i < squadLength; i++)
            {
                Allies[i] = center + Vector3.left * centralGap + Vector3.left * gap * i;
                Enemies[i] = center + Vector3.right * centralGap + Vector3.right * gap * i;
            }
        }

        public Vector3 Ally(int num)
        {
            if (num >= squadLength || num < 0)
            {
                Debug.LogError("Trying to get a wrong ally position");
                return Vector3.negativeInfinity;
            }

            return Allies[num];
        }

        public Vector3 Enemy(int num)
        {
            if (num >= squadLength || num < 0)
            {
                Debug.LogError("Trying to get a wrong enemy position");
                return Vector3.positiveInfinity;
            }

            return Enemies[num];
        }
    }
}