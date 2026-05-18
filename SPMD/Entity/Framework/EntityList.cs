using System.Collections.ObjectModel;

namespace Entity.Framework
{
  public class EntityList<T> : Collection<T>
    where T : EntityBase
  {
    
  }
}
