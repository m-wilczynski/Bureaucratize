using System;

namespace Bureaucratize.Common.Core.CommonDetails
{
    public class ObjectNotFoundById : IResultDetails
    {
        private readonly string _searchedObjectType;
        private readonly Guid _objectId;
        public string DetailsMessageKey => nameof(ObjectNotFoundById);

        public ObjectNotFoundById(Type searchedObjectType, Guid objectId)
        {
            _searchedObjectType = searchedObjectType.Name;
            _objectId = objectId;
        }

        public string GetDetails()
        {
            return $"Could not find object of type {_searchedObjectType} of id {_objectId}";
        }
    }
}
