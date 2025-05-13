namespace SMARTV3.Helpers
{
    public class ForcePackageWeightHelper
    {
        private readonly List<int> forcePackageIds;
        private List<List<int>> combineResult;
        private List<int> stack;
        private int n;
        private int k;

        public ForcePackageWeightHelper(List<int> forcePackageIds)
        {
            this.forcePackageIds = forcePackageIds;
            this.combineResult = new();
            this.stack = new();
        }

        public List<List<int>> CalculateCombinations()
        {
            List<List<int>> result = new();
            for (int i = 1; i < forcePackageIds.Count; i++)
            {
                combineResult = new();
                stack = new();
                n = forcePackageIds.Count;
                k = i;
                result.AddRange(Combinations());
            }
            result.Add(forcePackageIds);
            return result;
        }

        public List<List<int>> CalculateCombinationsPrimary(int primaryID)
        {
            List<List<int>> result = new();
            for (int i = 1; i < forcePackageIds.Count; i++)
            {
                combineResult = new();
                stack = new();
                n = forcePackageIds.Count;
                k = i;
                result.AddRange(CombinationsPrimary(primaryID));
            }
                result.Add(forcePackageIds);
            
            return result;
        }

        private void Combine(int currentNumber)
        {
            if (stack.Count == k)
            {
                List<int> tempStack = new();
                foreach (int i in stack)
                {
                    tempStack.Add(i);
                }
                combineResult.Add(tempStack);
                return;
            }
            if (currentNumber > n)
            {
                return;
            }
            stack.Add(forcePackageIds.ElementAt(currentNumber - 1));
            Combine(currentNumber + 1);
            stack.RemoveAt(stack.Count - 1);
            Combine(currentNumber + 1);
        }

        private void CombinePrimary(int currentNumber, int primaryID)
        {
            if (stack.Count == k)
            {
                List<int> tempStack = new();
                foreach (int i in stack)
                {
                    tempStack.Add(i);
                }
                if (tempStack.Exists(x => x.Equals(primaryID))) {
                    combineResult.Add(tempStack);
                }
                return;
            }
            if (currentNumber > n)
            {
                return;
            }
            stack.Add(forcePackageIds.ElementAt(currentNumber - 1));
            CombinePrimary(currentNumber + 1, primaryID);
            stack.RemoveAt(stack.Count - 1);
            CombinePrimary(currentNumber + 1,primaryID);
        }


        private List<List<int>> Combinations()
        {
            Combine(1);
            return combineResult;
        }
        private List<List<int>> CombinationsPrimary(int primaryID)
        {
            CombinePrimary(1,primaryID);
            return combineResult;
        }
    }
}