namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Classe base simples para todos os Value Objects.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Retorna os componentes que definem a igualdade do Value Object.
        /// </summary>
        protected abstract IEnumerable<object?> GetValues();

        public override bool Equals(object? obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;

            var other = (ValueObject)obj;
            using var thisComponents = GetValues().GetEnumerator();
            using var otherComponents = other.GetValues().GetEnumerator();

            while (thisComponents.MoveNext() && otherComponents.MoveNext())
            {
                var a = thisComponents.Current;
                var b = otherComponents.Current;

                if (a is null ^ b is null)
                    return false;

                if (a is not null && !a.Equals(b))
                    return false;
            }

            return !thisComponents.MoveNext() && !otherComponents.MoveNext();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var component in GetValues())
                    hash = hash * 23 + (component?.GetHashCode() ?? 0);
                return hash;
            }
        }

        public static bool operator ==(ValueObject? a, ValueObject? b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject? a, ValueObject? b)
            => !(a == b);
    }
}
