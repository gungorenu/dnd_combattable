/* 
copyright to Gungoren, Ugur
All rights reserved. Super Confidential.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;
using CombatTable.Models;

namespace CombatTable
{
    /// <summary>
    ///     This class contains methods for the CommandManager that help avoid memory leaks by
    ///     using weak references.
    /// </summary>
    internal static class CommandManagerHelper
    {
        /// <summary>
        /// Calls weak reference handlers
        /// </summary>
        /// <param name="handlers">Handler list</param>
        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                // Take a snapshot of the handlers before we call out to them since the handlers
                // could cause the array to me modified while we are reading it.

                var callees = new EventHandler[handlers.Count];
                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    var reference = handlers[i];
                    var handler = reference.Target as EventHandler;
                    if (handler == null)
                    {
                        // Clean up old handlers that have been collected
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }

                // Call the handlers that we snapshotted
                for (int i = 0; i < count; i++)
                {
                    EventHandler handler = callees[i];
                    handler(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Adds handlers to requery suggested list
        /// </summary>
        /// <param name="handlers">Handler list</param>
        internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    var handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested += handler;
                    }
                }
            }
        }

        /// <summary>
        /// Removes handlers from requery suggested list
        /// </summary>
        /// <param name="handlers">Handler list</param>
        internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    var handler = handlerRef.Target as EventHandler;
                    if (handler != null)
                    {
                        CommandManager.RequerySuggested -= handler;
                    }
                }
            }
        }

        /// <summary>
        /// Adds new weak reference handler
        /// </summary>
        /// <param name="handlers">Handler list</param>
        /// <param name="handler">Handler to be added</param>
        /// <param name="defaultListSize">Default list size for handler list</param>
        internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize = -1)
        {
            if (handlers == null)
            {
                handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        /// <summary>
        /// Removes weak reference handler from handler list
        /// </summary>
        /// <param name="handlers">Handler list</param>
        /// <param name="handler">Handler to be removed</param>
        internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    var existingHandler = reference.Target as EventHandler;
                    if ((existingHandler == null) || (existingHandler == handler))
                    {
                        // Clean up old handlers that have been collected
                        // in addition to the handler that is to be removed.
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }


    /// <summary>
    ///     This class allows delegating the commanding logic to methods passed as parameters,
    ///     and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Fields

        private readonly Action executeMethod;
        private readonly Func<bool> canExecuteMethod;
        private bool isAutomaticRequeryDisabled;
        private List<WeakReference> canExecuteChangedHandlers;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="executeMethod">Execute method</param>
        /// <param name="canExecuteMethod">Function for can execute method flag (default null)</param>
        /// <param name="isAutomaticRequeryDisabled">Flag, is automatic requery disabled (default false)</param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null, bool isAutomaticRequeryDisabled = false)
        {
            if (executeMethod == null)
                throw new Exception("Method is null!");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Method to determine if the command can be executed
        /// </summary>
        public bool CanExecute()
        {
            if (canExecuteMethod != null)
                return canExecuteMethod();
            return true;
        }

        /// <summary>
        ///     Execution of the command
        /// </summary>
        public void Execute()
        {
            if (executeMethod != null)
            {
                executeMethod();
                OnExecuted();
            }
        }

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return isAutomaticRequeryDisabled;
            }
            set
            {
                if (isAutomaticRequeryDisabled != value)
                {
                    if (value)
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(canExecuteChangedHandlers);
                    else
                        CommandManagerHelper.AddHandlersToRequerySuggested(canExecuteChangedHandlers);

                    isAutomaticRequeryDisabled = value;
                }
            }
        }

        /// <summary>
        ///     Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        ///     Raises the Executed event
        /// </summary>
        public void RaiseExecuted()
        {
            OnExecuted();
        }

        /// <summary>
        ///     Protected virtual method to raise Executed event
        /// </summary>
        protected virtual void OnExecuted()
        {
            if (Executed != null)
                Executed(this, new EventArgs());
        }

        /// <summary>
        ///     Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(canExecuteChangedHandlers);
        }

        /// <summary>
        /// Executed event handler, will be triggered after execution completes
        /// </summary>
        public event EventHandler Executed;

        #endregion

        #region ICommand Members

        /// <summary>
        ///     ICommand.CanExecuteChanged implementation
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested += value;

                CommandManagerHelper.AddWeakReferenceHandler(ref canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested -= value;

                CommandManagerHelper.RemoveWeakReferenceHandler(canExecuteChangedHandlers, value);
            }
        }

        /// <summary>
        /// Can execute method
        /// </summary>
        /// <param name="parameter">Parameter only for signature, ignored</param>
        /// <returns>True if can execute command</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Execute method
        /// </summary>
        /// <param name="parameter">Parameter only for signature, ignored</param>
        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        #endregion
    }

    public abstract class NewItemCommand<P> : Freezable, ICommand
        where P: NodeContainer

    {
        public NewItemCommand()
        {
            Binding pnt = new Binding("DataContext");
            pnt.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeViewItem), 1);
            pnt.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ParentProperty, pnt);
        }

        public object Parent
        {
            get { return (object)GetValue(ParentProperty); }
            set { SetValue(ParentProperty, value); }
        }

        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(object), typeof(NewItemCommand<P>), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override Freezable CreateInstanceCore()
        {
            return this;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            AddNewItem((P)Parent);
        }

        protected abstract void AddNewItem(P parent);
    }

    public abstract class DeleteItemCommand<C> : Freezable, ICommand
    {
        public DeleteItemCommand()
        {
            Binding pnt = new Binding("DataContext");
            pnt.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeViewItem), 1);
            pnt.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ChildProperty, pnt);
        }

        public object Child
        {
            get { return (object)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(object), typeof(DeleteItemCommand<C>), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            DeleteItem((C)Child);
        }

        protected abstract void DeleteItem(C child);
    }

}
