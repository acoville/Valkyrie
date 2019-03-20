using System;
using Xamarin.Forms;

/*====================================================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * 
 * UpSkilled ICT401515 / Core Infrastructure Mobile Project
 * 
 * credit to David Britch
 * https://developer.xamarin.com/samples/xamarin-forms/behaviors/eventtocommandbehavior/
 * 
 * =================================================================================*/

namespace DarkValkyrie.Behaviors
{
    public class BehaviorBase<T> : Behavior<T> where T : BindableObject
    {
        public T AssociatedObject { get; private set; }

        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;

            if (bindable.BindingContext != null)
            {
                BindingContext = bindable.BindingContext;
            }

            bindable.BindingContextChanged += OnBindingContextChanged;
        }

        //=============================================================

        protected override void OnDetachingFrom(T bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            AssociatedObject = null;
        }

        //=============================================================

        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        //=============================================================

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}
