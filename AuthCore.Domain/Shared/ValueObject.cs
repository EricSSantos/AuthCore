namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Classe base para todos os Value Objects do domínio.
    /// Define regras de igualdade baseadas em seus valores internos.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Retorna os componentes que definem a igualdade do Value Object.
        /// </summary>
        protected abstract IEnumerable<object?> GetValues();

        /// <summary>
        /// Compara dois Value Objects com base em seus componentes.
        /// </summary>
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

        /// <summary>
        /// Gera um código de hash com base nos componentes do Value Object.
        /// </summary>
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

        /// <summary>
        /// Compara dois Value Objects para verificar se são iguais.
        /// </summary>
        public static bool operator ==(ValueObject? a, ValueObject? b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Compara dois Value Objects para verificar se são diferentes.
        /// </summary>
        public static bool operator !=(ValueObject? a, ValueObject? b)
            => !(a == b);
    }
}
