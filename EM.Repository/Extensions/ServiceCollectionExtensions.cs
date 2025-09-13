using System.Reflection;
using EM.Domain.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace EM.Repository.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFromAssembly<TInterface>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            return services.AddFromAssembly(typeof(TInterface).Assembly, lifetime);
        }

        public static IServiceCollection AddFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var types = assembly.GetTypes();
            
            var interfaces = types.Where(t => 
                t.IsInterface && 
                !t.IsGenericTypeDefinition && 
                t.IsPublic)
                .ToList();
            
            var classes = types.Where(t => 
                t.IsClass && 
                !t.IsAbstract && 
                !t.IsGenericTypeDefinition && 
                t.IsPublic && 
                t.GetConstructors().Any(c => c.IsPublic))
                .ToList();

            int registrosRealizados = 0;

            foreach (var interfaceType in interfaces)
            {
                var implementacoes = classes.Where(c => interfaceType.IsAssignableFrom(c)).ToList();
                
                if (implementacoes.Count == 1)
                {
                    var implementacao = implementacoes.First();
                    
                    var serviceDescriptor = new ServiceDescriptor(interfaceType, implementacao, lifetime);
                    services.Add(serviceDescriptor);
                    
                    registrosRealizados++;
                }
                else if (implementacoes.Count > 1)
                {
                    
                    foreach (var implementacao in implementacoes)
                    {
                        var serviceDescriptor = new ServiceDescriptor(interfaceType, implementacao, lifetime);
                        services.Add(serviceDescriptor);
                        Console.WriteLine($"   ➤ {implementacao.Name}");
                    }
                    registrosRealizados += implementacoes.Count;
                }
            }

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetExecutingAssembly() };
            }

            foreach (var assembly in assemblies)
            {
                var repositoryTypes = assembly.GetTypes()
                    .Where(type => IsRepositoryType(type))
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .ToList();

                foreach (var repositoryType in repositoryTypes)
                {
                    services.AddScoped(repositoryType);
                    
                    var repositoryInterfaces = repositoryType.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>))
                        .ToList();

                    foreach (var repositoryInterface in repositoryInterfaces)
                    {
                        services.AddScoped(repositoryInterface, repositoryType);
                    }
                }
            }

            return services;
        }

        private static bool IsRepositoryType(Type type)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>)) ||
                type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase);
        }

        public static IServiceCollection AddByConvention(this IServiceCollection services, Assembly assembly, string interfacePrefix = "I", ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var types = assembly.GetTypes();
            
            var interfaces = types.Where(t => 
                t.IsInterface && 
                t.Name.StartsWith(interfacePrefix) &&
                t.IsPublic)
                .ToList();

            foreach (var interfaceType in interfaces)
            {
                var expectedClassName = interfaceType.Name.Substring(interfacePrefix.Length);
                
                var implementationType = types.FirstOrDefault(t => 
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.Name == expectedClassName &&
                    interfaceType.IsAssignableFrom(t));

                if (implementationType != null)
                {
                    services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
                }
                else
                {
                    Console.WriteLine($"Não encontrada implementação para: {interfaceType.Name} (esperado: {expectedClassName})");
                }
            }

            return services;
        }
    }
}