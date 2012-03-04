using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aljeida.VisualStudio.Core
{
    public class Relationship
    {
        public Type Type1 { get; set; }
        public Type Type2 { get; set; }
        public RelationshipType RelationshipType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Relationship(Type type1, Type type2, RelationshipType relationshipType)
        {
            Type1 = type1;
            Type2 = type2;
            RelationshipType = relationshipType;
        }
    }
}
