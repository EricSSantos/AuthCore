using System.Net;

namespace AuthCore.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }

        public DomainException(string message)
            : base(message)
        {
            Errors = new[] { message };
        }

        public DomainException(IEnumerable<string> errors, Exception? innerException = null)
            : base(errors != null && errors.Any() ? string.Join("; ", errors) : "Violação na regra de negócio.", innerException)
        {
            Errors = errors?.ToList() ?? new List<string>();
        }

        public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public virtual string Title => "Violação na regra de negócio.";
    }
}
