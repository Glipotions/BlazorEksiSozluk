using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Extensions
{
    public static class Registration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            /// çalışan assembly yani web apiye bağlı olan tüm class library leri ekler
            var assm = Assembly.GetExecutingAssembly();

            /// dependency injection paketlerini kurmamızın nedeni tek tek uğraşmak yerine
            /// direkt tüm assembly de service i kullananların aktif edilmesini sağlamaktır.
            services.AddMediatR(assm);
            services.AddAutoMapper(assm);
            services.AddValidatorsFromAssembly(assm);

            return services;
        }
    }
}
