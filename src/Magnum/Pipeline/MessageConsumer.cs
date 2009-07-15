namespace FunctionalBits.Pipeline
{
    public delegate void MessageConsumer<T>(T message);

    public delegate void Unsubscriber();
}