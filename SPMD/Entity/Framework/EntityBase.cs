using System;

namespace Entity.Framework
{
  public abstract class EntityBase
  {
    public object ID { get; set; }

    protected EntityBase(object id)
    {
      ID = id;
    }
  }
}
