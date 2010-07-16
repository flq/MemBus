using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MemBus
{
    public static class RespondExtension
    {
        public static bool TryInvoke(this object target, Action<dynamic> selector)
        {
            var dynExists = new DynamicExists(target);
            selector(dynExists);
            if (dynExists.OperationExists)
                selector(target);
            return dynExists.OperationExists;
        }

        public static bool RespondsTo(this object target, Func<dynamic,object> selector)
        {
            var dynExists = new DynamicExists(target);
            selector(dynExists);
            return dynExists.OperationExists;
        }

        public static bool RespondsTo(this object target, Action<dynamic> selector)
        {
            var dynExists = new DynamicExists(target);
            selector(dynExists);
            return dynExists.OperationExists;
        }

        private class DynamicExists : DynamicObject
        {
            private readonly object instance;
            
            public DynamicExists(object instance)
            {
                this.instance = instance;
            }

            public bool OperationExists { get; private set; }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var possibleMembers =
                    from mi in instance.GetType().GetMember(binder.Name).OfType<MethodInfo>()
                    let parms = mi.GetParameters()
                    where parms.Length == args.Length
                    let correlates = parms.Zip(args, (pi, o) => (o == null && pi.ParameterType.IsClass) || ( o != null && pi.ParameterType.Equals(o.GetType()))).All(truth => truth)
                    where correlates
                    select mi;

                OperationExists = possibleMembers.Count() == 1; //Expecting 1 or none as method overloading rules would be violated
                result = null;

                return true; // We always "find" a member
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var possibleMembers = from pi in instance.GetType().GetMember(binder.Name).OfType<PropertyInfo>()
                    where pi.GetSetMethod() != null && 
                          (
                            (pi.PropertyType.IsClass && value == null) || 
                            (value != null && pi.PropertyType.Equals(value.GetType()))
                          )
                    select pi;
                OperationExists = possibleMembers.Count() == 1;
                return true; // We always "find" a member
                
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var possibleMembers = instance.GetType().GetMember(binder.Name);
                OperationExists = possibleMembers.Length > 0;
                result = null;
                return true; // We always "find" a member
            }
        }
    }
}