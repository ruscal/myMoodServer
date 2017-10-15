using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Discover.DomainModel
{
    /// <summary>
    /// The marker interface that is used to identify domain event classes
    /// </summary>
    public interface IDomainEvent { }

    /// <summary>
    /// A generic interface that classes may implement in order to receive and process specific types of domain events
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IHandle<TEvent> where TEvent : IDomainEvent
    {
        void Handle(TEvent e);
    }

    /// <summary>
    /// This class provides static methods which may be used to raise domain events from within domain model classes,
    /// and discover event handler classes
    /// </summary>
    public static class DomainEvents
    {
        public static IEventDispatcher Dispatcher { get; private set; }
        
        static DomainEvents()
        {
            Dispatcher = new NullEventDispatcher();
        }

        public static void SetDispatcher(IEventDispatcher eventDispatcher)
        {
            Dispatcher = eventDispatcher;
        }

        /// <summary>
        /// Raises the given domain event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="e"></param>
        public static void Raise<TEvent>(TEvent e) where TEvent : IDomainEvent
        {
            Dispatcher.Dispatch<TEvent>(e);
        }

        /// <summary>
        /// Locates all domain event types in the current AppDomain
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEvents()
        {
            return FindDomainEvents(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));
        }

        /// <summary>
        /// Locates all domain event types in the given assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEvents(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => typeof(IDomainEvent).IsAssignableFrom(t));
        }

        /// <summary>
        /// Locates all domain event types in the given set of assemblies
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEvents(IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(IDomainEvent).IsAssignableFrom(t));
        }

        /// <summary>
        /// Locates all types in the current AppDomain which handle the given domain event type
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEventHandlersFor<TEvent>() where TEvent : IDomainEvent
        {
            return FindDomainEventHandlersFor<TEvent>(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));
        }

        /// <summary>
        /// Locates all types in the given assembly which handle the given domain event type
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEventHandlersFor<TEvent>(Assembly assembly) where TEvent : IDomainEvent
        {
            return assembly.GetTypes().Where(t => typeof(IHandle<TEvent>).IsAssignableFrom(t));
        }

        /// <summary>
        /// Locates all types in the given set of assemblies which handle the given domain event type
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindDomainEventHandlersFor<TEvent>(IEnumerable<Assembly> assemblies) where TEvent : IDomainEvent
        {
            return assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(IHandle<TEvent>).IsAssignableFrom(t));
        }

        /// <summary>
        /// Locates all types in the current AppDomain which handle any domain event type defined in the current AppDomain
        /// </summary>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlers()
        {
            return FindDomainEventHandlers(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));
        }

        /// <summary>
        /// Locates all types in the given assembly which handle any domain event type defined in the current AppDomain
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlers(Assembly assembly)
        {
            return FindDomainEventHandlers(new Assembly[] { assembly });
        }

        /// <summary>
        /// Locates all types in the given set of assemblies which handle any domain event type defined in the current AppDomain
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlers(IEnumerable<Assembly> assemblies)
        {
            return FindDomainEventHandlersFor(FindDomainEvents(), assemblies);
        }

        /// <summary>
        /// Locates all types in the current AppDomain which handle the any of the given set of domain event types
        /// </summary>
        /// <param name="eventTypes"></param>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlersFor(IEnumerable<Type> eventTypes)
        {
            return FindDomainEventHandlersFor(eventTypes, AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));
        }

        /// <summary>
        /// Locates all types in the given assembly which handle any of the given set of domain event types
        /// </summary>
        /// <param name="eventTypes"></param>
        /// <param name="assembly"></param>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlersFor(IEnumerable<Type> eventTypes, Assembly assembly)
        {
            return FindDomainEventHandlersFor(eventTypes, new Assembly[] { assembly });
        }

        /// <summary>
        /// Locates all types in the given set of assemblies which handle any of the given set of domain event types
        /// </summary>
        /// <param name="eventTypes"></param>
        /// <param name="assemblies"></param>
        /// <returns>A set of key-value pairs which relate event types (Key) to the set of related event handler types that were found (Value)</returns>
        public static IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> FindDomainEventHandlersFor(IEnumerable<Type> eventTypes, IEnumerable<Assembly> assemblies)
        {
            var eventHandlerTypes = (from t in assemblies.SelectMany(a => a.GetTypes())
                                     where t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>))
                                     select t)
                                .ToArray();

            foreach (var eventType in eventTypes)
            {
                var handlers = eventHandlerTypes.Where(t => typeof(IHandle<>).MakeGenericType(eventType).IsAssignableFrom(t)).ToArray();

                if (handlers.Any())
                {
                    yield return new KeyValuePair<Type, IEnumerable<Type>>(eventType, handlers);
                }
            }
        }
    }

    /// <summary>
    /// The interface which a domain event dispatch implementation must support
    /// </summary>
    public interface IEventDispatcher
    {
        void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent;
    }

    /// <summary>
    /// A dummy/stub domain event dispatch implementation
    /// </summary>
    public sealed class NullEventDispatcher : IEventDispatcher
    {
        public void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent
        {
            System.Diagnostics.Debug.WriteLine(string.Format("*** Domain event raised - {0} ***", eventToDispatch.ToString()));
        }
    }

    /// <summary>
    /// A domain event dispatch implementation that uses the current System.Web.Mvc.DependencyResolver to locate registered event handlers
    /// </summary>
    public sealed class MvcDependencyResolverEventDispatcher : IEventDispatcher
    {
        public void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent
        {
            foreach (var handler in System.Web.Mvc.DependencyResolver.Current.GetServices(typeof(IHandle<TEvent>)).Cast<IHandle<TEvent>>())
            {
                handler.Handle(eventToDispatch);
            }
        }
    }
}
