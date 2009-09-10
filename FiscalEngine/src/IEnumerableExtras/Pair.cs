namespace IEnumerableExtras
{
    /// <summary>
    /// Represents a pair of items of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Pair< TValue >
    {
        private readonly TValue _a;
        private readonly TValue _b;

        /// <summary>
        /// Initialize an instance of <see cref="Pair{TValue}"/> with two
        /// items of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Pair( TValue a, TValue b )
        {
            _a = a;
            _b = b;
        }

        /// <summary>
        /// Gets first (left) item.
        /// </summary>
        public TValue A
        {
            get
            {
                return _a;
            }
        }

        /// <summary>
        /// Gets second (right) item.
        /// </summary>
        public TValue B
        {
            get
            {
                return _b;
            }
        }
    }
}