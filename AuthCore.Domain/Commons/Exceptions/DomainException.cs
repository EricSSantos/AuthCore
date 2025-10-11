using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public class DomainException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public virtual HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.BadRequest; }
        }

        public virtual string Title
        {
            get { return "Violação na regra de negócio."; }
        }

        public DomainException(string message)
            : base(message)
        {
            Errors = new[] { message };
        }

        public DomainException(IEnumerable<string> errors, Exception? innerException = null)
            : base(
                errors != null && errors.Any()
                    ? string.Join("; ", errors)
                    : "Violação na regra de negócio.",
                innerException
            )
        {
            Errors = errors?.ToList() ?? new List<string>();
        }
    }
}
