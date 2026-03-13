using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    public class WeightedRandom
    {
        private readonly int[] _counts;

        public WeightedRandom(int count)
        {
            _counts = new int[count];
        }

        public int GetNextValue()
        {
            // Calculate weights inversely proportional to counts
            float[] weights = _counts.Select(c => 1f / (c + 1f)).ToArray();

            // Normalize weights to sum to 1
            float totalWeight = weights.Sum();
            float[] probabilities = weights.Select(w => w / totalWeight).ToArray();

            // Generate a random value based on the probabilities
            float randomValue = Random.value; // Random value between 0 and 1
            float cumulativeProbability = 0f;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue <= cumulativeProbability)
                {
                    _counts[i]++; // Increment the count for the selected value
                    return i;
                }
            }

            return 0; // Fallback (shouldn't happen)
        }
    }
}