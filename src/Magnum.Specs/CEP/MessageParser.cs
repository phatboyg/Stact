namespace Magnum.Specs.CEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class Bob
    {
        [Test]
        public void aaa()
        {
            var eventStream = new List<object>()
                              {
                                  new object(),
                                  new int(),
                                  new object(),
                                  new int()
                              };
            var channel = new ListChannel<object>(eventStream);

            var messages = new MessageParser().Parse(channel);

            Assert.AreEqual(2, messages.Count());
        }
    }

    public class MessageParser
    {
        public MessageParser()
        {
            InterestingMessages = Msg<int>();
            Element =from m in AnyMessage 
                     from i in InterestingMessages
                     select i;
            All = Element;
        }

        public IEnumerable<object> Parse(Channel<object> channel)
        {
            Result<object> result = All(channel);
            while(result != null)
            {
                yield return result.Value;

                result = All(result.Rest);
            }
        }

        public Parser<object> AnyMessage
        {
            get
            {
                return o =>
                {
                    if (o.HasMessages)
                    {
                        return new Result<object>(o.NextMessage, o.Tail);
                    }
                    else
                    {
                        return null;
                    }
                };
            }
        }

        public Parser<object> Msg<T>()
        {
            return from m in AnyMessage
                   where m is T
                   select m;
        }

        public Parser<object> Msg<T>(Predicate<object> predicate)
        {
            return from m in AnyMessage
                   where predicate(m)
                   select m;
        }

        public Parser<object> All { get; private set; }
        public Parser<object> Element { get; private set; }
        public Parser<object> InterestingMessages { get; private set; }
    }

    public static class MonadicExtensions
    {
        public static Parser<INPUT> Where<INPUT>(this Parser<INPUT> parser, Func<INPUT, bool> pred)
        {
            return input =>
            {
                Result<INPUT> result = parser(input);
                if (result == null || !pred(result.Value))
                    return null;

                return result;
            };
        }
        public static Parser<INPUT> Select<INPUT>(this Parser<INPUT> parser, Func<INPUT, INPUT> selector)
        {
            return input =>
            {
                Result<INPUT> result = parser(input);
                if (result == null)
                    return null;

                return new Result<INPUT>(selector(result.Value), result.Rest);
            };
        }

        public static Parser<INPUT> SelectMany<INPUT>(this Parser<INPUT> parser, Func<INPUT, Parser<INPUT>> selector, Func<INPUT, INPUT, INPUT> projector)
        {
            return input =>
            {
                Result<INPUT> result = parser(input);
                if (result == null)
                    return null;

                INPUT val = result.Value;
                Result<INPUT> nextResult = selector(val)(result.Rest);
                if (nextResult == null)
                    return null;

                return new Result<INPUT>(projector(val, nextResult.Value), nextResult.Rest);
            };
        }
    }
    public interface Channel<OF>
    {
        bool HasMessages { get; }
        OF NextMessage { get; }
        Channel<OF> Tail { get; }
    }

    public class ListChannel<OF> :
        Channel<OF>
    {
        List<OF> _messages;

        public ListChannel(List<OF> messages)
        {
            _messages = messages;
        }

        public bool HasMessages
        {
            get
            {
                return _messages.Count > 0;
            }
        }

        public OF NextMessage
        {
            get
            {
                return _messages[0];
            }
        }

        public Channel<OF> Tail
        {
            get
            {
                var messages = _messages;
                messages.RemoveAt(0);
                return new ListChannel<OF>(messages);
            }
        }
    }
    public delegate Result<INPUT> Parser<INPUT>(Channel<INPUT> input);

    public class Result<INPUT>
    {
        public Result(INPUT value, Channel<INPUT> rest)
        {
            Value = value;
            Rest = rest;
        }

        public INPUT Value { get; private set; }
        public Channel<INPUT> Rest { get; private set; }
    }
}