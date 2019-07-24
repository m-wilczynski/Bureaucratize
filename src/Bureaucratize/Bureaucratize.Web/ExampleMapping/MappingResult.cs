using System.Collections.Generic;

namespace Bureaucratize.Web.ExampleMapping
{
    public class MappingResult<T>
    {
        public string Scope { get; set; }
        public T Result { get; set; }
        public bool WasSuccessful { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
