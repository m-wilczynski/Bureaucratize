namespace Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts
{
    public class SaveDomainObject<TObject> : ICommand where TObject : Identifiable
    {
        public TObject ObjectToSave { get; set; }
    }

    public static class SaveDomainObjectExtensions
    {
        public static SaveDomainObject<TObject> AsSaveCommand<TObject>(this TObject obj)
            where TObject : Identifiable
        {
            return new SaveDomainObject<TObject>
            {
                ObjectToSave = obj
            };
        }
    }
}
