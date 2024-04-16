using System;

public interface IBuff : IComparable<IBuff>
{
    int ID { get; }
    Attribute.Type AttribType { get; }
}