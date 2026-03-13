using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// The Injector class is responsible for dependency injection in the application.
    /// It provides methods to register dependencies, inject dependencies into objects, validate dependencies, and clear dependencies.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Injector : PersistentSingleton<Injector>
    {
        [SerializeField] private bool autoInjectOnAwake = true;

        private const BindingFlags kBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private readonly Dictionary<Type, object> _registry = new();

        protected override void Awake()
        {
            base.Awake();

            if (instance != this || !autoInjectOnAwake) return;

            var monoBehaviours = FindAllMonoBehaviours();
            AutoRegister(monoBehaviours);
            AutoInject(monoBehaviours);
        }

        /// <summary>
        /// Automatically injects dependencies into all MonoBehaviours found in the scene.
        /// </summary>
        public void AutoInjectMonoBehaviours()
        {
            var monoBehaviours = FindAllMonoBehaviours();
            AutoInject(monoBehaviours);
        }

        /// <summary>
        /// Registers an instance of type T in the injector's registry.
        /// </summary>
        /// <typeparam name="T">The type of the instance being registered.</typeparam>
        /// <param name="instance">The instance to be registered.</param>
        public void Register<T>(T instance)
        {
            _registry[typeof(T)] = instance;
        }

        /// <summary>
        /// Injects dependencies into the specified instance by invoking the necessary injection methods.
        /// </summary>
        /// <param name="instance">The instance to inject dependencies into.</param>
        public void Inject(object instance)
        {
            InjectFields(instance);
            InjectMethods(instance);
            InjectProperties(instance);
        }

        /// <summary>
        /// Validates the dependencies of the current object by checking if all required dependencies are provided.
        /// If any dependencies are missing, an error message is logged. Otherwise, a success message is logged.
        /// </summary>
        public void ValidateDependencies()
        {
            var monoBehaviours = FindAllMonoBehaviours();
            var providedDependencies = GetProvidedDependencies(monoBehaviours);

            var invalidMethods = monoBehaviours
            .SelectMany(mono => mono.GetType().GetMethods(kBindingFlags), (mono, method) => new { mono, method })
            .Where(@t => Attribute.IsDefined(@t.method, typeof(InjectAttribute)))
            .Where(@t => @t.method.GetParameters().Any(parameter => !providedDependencies.Contains(parameter.ParameterType)))
            .Select(@t => $"[DILite][Validation] {t.mono.GetType().Name} is missing dependency: {@t.method.GetParameters().First().ParameterType.Name} on GameObject: {@t.mono.gameObject.name}");

            var invalidFields = monoBehaviours
            .SelectMany(mono => mono.GetType().GetFields(kBindingFlags), (mono, field) => new { mono, field })
            .Where(@t => Attribute.IsDefined(@t.field, typeof(InjectAttribute)))
            .Where(@t => !providedDependencies.Contains(@t.field.FieldType) && t.field.GetValue(t.mono) == null)
            .Select(@t => $"[DILite][Validation] {t.mono.GetType().Name} is missing dependency: {@t.field.FieldType.Name} on GameObject: {@t.mono.gameObject.name}");

            var invalidProperties = monoBehaviours
            .SelectMany(mono => mono.GetType().GetProperties(kBindingFlags), (mono, property) => new { mono, property })
            .Where(@t => Attribute.IsDefined(@t.property, typeof(InjectAttribute)))
            .Where(@t => !providedDependencies.Contains(@t.property.PropertyType) && t.property.GetValue(t.mono) == null)
            .Select(@t => $"[DILite][Validation] {t.mono.GetType().Name} is missing dependency: {@t.property.PropertyType.Name} on GameObject: {@t.mono.gameObject.name}");

            var invalidDependenciesArray = invalidMethods.Concat(invalidFields).Concat(invalidProperties).ToArray();
            if (invalidDependenciesArray.Length > 0)
            {
                Debug.LogError($"[DILite][Validation] Found {invalidDependenciesArray.Length} invalid dependencies: {string.Join(", ", invalidDependenciesArray)}");
            }
            else
            {
                Debug.Log("[DILite][Validation] All dependencies are valid");
            }
        }

        private void AutoRegister(MonoBehaviour[] monoBehaviours)
        {
            foreach (var mono in monoBehaviours)
            {
                Register(mono);
            }
        }

        private void AutoInject(MonoBehaviour[] monoBehaviours)
        {
            var injectables = monoBehaviours.Where(IsInjectable);
            foreach (var injectable in injectables)
            {
                Inject(injectable);
            }
        }

        private void Register(MonoBehaviour monoBehaviour)
        {
            RegisterMethods(monoBehaviour);
            RegisterProperties(monoBehaviour);
            RegisterFields(monoBehaviour);
        }

        private void RegisterMethods(MonoBehaviour monoBehaviour)
        {
            var methods = monoBehaviour.GetType().GetMethods(kBindingFlags).Where(method => Attribute.IsDefined(method, typeof(ProvideAttribute)));
            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var value = method.Invoke(monoBehaviour, null);
                if (value == null)
                {
                    Debug.LogError($"[DILite] Failed to provide {returnType.Name} from {monoBehaviour.GetType().Name}");
                    continue;
                }

                _registry.Add(returnType, value);
            }
        }

        private void RegisterFields(MonoBehaviour monoBehaviour)
        {
            var fields = monoBehaviour.GetType().GetFields(kBindingFlags).Where(field => Attribute.IsDefined(field, typeof(ProvideAttribute)));
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var value = field.GetValue(monoBehaviour);
                if (value == null)
                {
                    Debug.LogWarning($"[DILite] {fieldType.Name} from {monoBehaviour.GetType().Name} is null");
                    continue;
                }

                _registry.Add(fieldType, value);
            }
        }

        private void RegisterProperties(MonoBehaviour monoBehaviour)
        {
            var properties = monoBehaviour.GetType().GetProperties(kBindingFlags).Where(property => Attribute.IsDefined(property, typeof(ProvideAttribute)));
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var value = property.GetValue(monoBehaviour);
                if (value == null)
                {
                    Debug.LogError($"[DILite] Failed to provide {propertyType.Name} from {monoBehaviour.GetType().Name}");
                    continue;
                }

                _registry.Add(propertyType, value);
            }
        }

        private void InjectMethods(object instance)
        {
            var type = instance.GetType();
            var methods = GetAllMethods(type).Where(method => Attribute.IsDefined(method, typeof(InjectAttribute)));
            foreach (var method in methods)
            {
                var requiredParameters = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                var resolvedParameters = requiredParameters.Select(Resolve).ToArray();
                if (resolvedParameters.Any(parameter => parameter == null))
                {
                    Debug.LogError($"[DILite] Failed to resolve parameters for method {method.Name} in {type.Name}");
                    continue;
                }

                method.Invoke(instance, resolvedParameters);
            }
        }

        private void InjectFields(object instance)
        {
            var type = instance.GetType();
            var fields = GetAllFields(type).Where(field => Attribute.IsDefined(field, typeof(InjectAttribute)));
            foreach (var field in fields)
            {
                var fieldType = field.FieldType;
                var value = Resolve(fieldType);
                if (value == null)
                {
                    Debug.LogError($"[DILite] Failed to resolve {fieldType.Name} for field {field.Name} in {type.Name} - GameObject: {(instance as MonoBehaviour).name}");
                    continue;
                }

                field.SetValue(instance, value);
            }
        }

        private void InjectProperties(object instance)
        {
            var type = instance.GetType();
            var properties = GetAllProperties(type).Where(property => Attribute.IsDefined(property, typeof(InjectAttribute)));
            foreach (var property in properties)
            {
                if (property.GetValue(instance) != null)
                {
                    Debug.LogWarning($"[DILite] Property {property.Name} is already assigned in {type.Name}");
                    continue;
                }

                var propertyType = property.PropertyType;
                var value = Resolve(propertyType);
                if (value == null)
                {
                    Debug.LogError($"[DILite] Failed to resolve {propertyType.Name} for property {property.Name} in {type.Name}");
                    continue;
                }

                property.SetValue(instance, value);
            }
        }

        private object Resolve(Type type)
        {
            _registry.TryGetValue(type, out var value);
            return value;
        }

        private HashSet<Type> GetProvidedDependencies(IEnumerable<MonoBehaviour> monoBehaviours)
        {
            var dependencies = new HashSet<Type>();
            foreach (var mono in monoBehaviours)
            {
                var attributes = mono.GetType().GetMembers(kBindingFlags).Where(member => Attribute.IsDefined(member, typeof(ProvideAttribute)));
                foreach (var attribute in attributes)
                {
                    if (attribute is MethodInfo method)
                    {
                        var returnType = method.ReturnType;
                        dependencies.Add(returnType);
                    }
                    else if (attribute is FieldInfo field)
                    {
                        var fieldType = field.FieldType;
                        dependencies.Add(fieldType);
                    }
                    else if (attribute is PropertyInfo property)
                    {
                        var propertyType = property.PropertyType;
                        dependencies.Add(propertyType);
                    }
                }
            }

            return dependencies;
        }

        private MonoBehaviour[] FindAllMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        private bool IsInjectable(MonoBehaviour mono)
        {
            var members = mono.GetType().GetMembers(kBindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        private IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null) return Enumerable.Empty<FieldInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        private IEnumerable<PropertyInfo> GetAllProperties(Type type)
        {
            if (type == null) return Enumerable.Empty<PropertyInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetProperties(flags).Concat(GetAllProperties(type.BaseType));
        }

        private IEnumerable<MethodInfo> GetAllMethods(Type type)
        {
            if (type == null) return Enumerable.Empty<MethodInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetMethods(flags).Concat(GetAllMethods(type.BaseType));
        }
    }
}
