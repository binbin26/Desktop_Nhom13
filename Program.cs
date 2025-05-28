﻿using Desktop_Nhom13.DAL;
using Desktop_Nhom13.Forms.Auth;
using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Desktop_Nhom13.BLL;
//using Desktop_Nhom13.Forms.Teacher;
using Desktop_Nhom13.Models.Users;

namespace Desktop_Nhom13
{
    static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show("Không thể kết nối đến database!");
                return;
            }
            Application.Run(new LoginForm());

        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserContext, UserContext>();
            services.AddTransient<CourseDAL>();
            services.AddTransient<CourseBLL>();
        }
    }
}

