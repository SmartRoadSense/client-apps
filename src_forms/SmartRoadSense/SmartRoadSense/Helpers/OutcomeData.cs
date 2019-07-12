using System;
namespace SmartRoadSense
{
    public enum OutcomeDataEvents { onlineSuccess, backupSuccess, error };

    public class OutcomeData<T, E> where E : Exception
    {
        OutcomeDataEvents _event;
        T _element;
        E _error;

        public OutcomeData(T element, bool online)
        {
            if (element.GetType() == typeof(Outcome<T, E>))
            {
                // from generic outcome
                var outcome = element as Outcome<T, E>;
                if (outcome.Event == OutcomeEvents.success)
                    if (online) _event = OutcomeDataEvents.onlineSuccess;
                    else _event = OutcomeDataEvents.backupSuccess;
                else
                    _error = outcome.Error;
                _element = outcome.Element;
            }
            else
            {
                if (online) _event = OutcomeDataEvents.onlineSuccess;
                else _event = OutcomeDataEvents.backupSuccess;
                _element = element;
            }
        }

        public OutcomeData(Outcome<T, E> outcome, bool online)
        {
            if (outcome.Event == OutcomeEvents.success)
                if (online) _event = OutcomeDataEvents.onlineSuccess;
                else _event = OutcomeDataEvents.backupSuccess;
            else
                _error = outcome.Error;
            _element = outcome.Element;
        }


        public OutcomeData(E error)
        {
            _event = OutcomeDataEvents.error;
            _error = error;
        }

        public OutcomeDataEvents Event
        {
            get { return _event; }
        }

        public T Element
        {
            get { return _element; }
        }

        public E Error
        {
            get { return _error; }
        }
    }

    public class OutcomeData<E> where E : Exception
    {
        OutcomeDataEvents _event;
        E _error;

        public OutcomeData(bool online)
        {
            if (online) _event = OutcomeDataEvents.onlineSuccess;
            else _event = OutcomeDataEvents.backupSuccess;
        }

        public OutcomeData(E error)
        {
            _event = OutcomeDataEvents.error;
            _error = error;
        }

        public OutcomeDataEvents Event
        {
            get { return _event; }
        }

        public E Error
        {
            get { return _error; }
        }
    }
}
