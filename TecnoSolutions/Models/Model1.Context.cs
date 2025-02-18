﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TecnoSolutions.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BD_14_02Entities3 : DbContext
    {
        public BD_14_02Entities3()
            : base("name=BD_14_02Entities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CREW> CREW { get; set; }
        public virtual DbSet<CREW_PERSON> CREW_PERSON { get; set; }
        public virtual DbSet<PERSON> PERSON { get; set; }
        public virtual DbSet<PRODUCT> PRODUCT { get; set; }
        public virtual DbSet<PRODUCT_PERSON> PRODUCT_PERSON { get; set; }
        public virtual DbSet<QUOTE> QUOTE { get; set; }
        public virtual DbSet<QUOTE_PRODUCT> QUOTE_PRODUCT { get; set; }
        public virtual DbSet<QUOTE_SERVICE> QUOTE_SERVICE { get; set; }
        public virtual DbSet<ROLE> ROLE { get; set; }
        public virtual DbSet<SERVICE> SERVICE { get; set; }
        public virtual DbSet<STATE> STATE { get; set; }
    
        public virtual int sp_UserRegistered(string firstName, string lastName, string document, string phone, string address, string department, string city, string email, string password, ObjectParameter registered, ObjectParameter message, Nullable<int> idRol)
        {
            var firstNameParameter = firstName != null ?
                new ObjectParameter("FirstName", firstName) :
                new ObjectParameter("FirstName", typeof(string));
    
            var lastNameParameter = lastName != null ?
                new ObjectParameter("LastName", lastName) :
                new ObjectParameter("LastName", typeof(string));
    
            var documentParameter = document != null ?
                new ObjectParameter("Document", document) :
                new ObjectParameter("Document", typeof(string));
    
            var phoneParameter = phone != null ?
                new ObjectParameter("Phone", phone) :
                new ObjectParameter("Phone", typeof(string));
    
            var addressParameter = address != null ?
                new ObjectParameter("Address", address) :
                new ObjectParameter("Address", typeof(string));
    
            var departmentParameter = department != null ?
                new ObjectParameter("Department", department) :
                new ObjectParameter("Department", typeof(string));
    
            var cityParameter = city != null ?
                new ObjectParameter("City", city) :
                new ObjectParameter("City", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var idRolParameter = idRol.HasValue ?
                new ObjectParameter("IdRol", idRol) :
                new ObjectParameter("IdRol", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_UserRegistered", firstNameParameter, lastNameParameter, documentParameter, phoneParameter, addressParameter, departmentParameter, cityParameter, emailParameter, passwordParameter, registered, message, idRolParameter);
        }
    
        public virtual int sp_UserRegisteredEmployees(Nullable<int> idRole, string firstName, string lastName, string document, string phone, string address, string department, string city, string email, string password, string arl, string eps, string position, ObjectParameter registered, ObjectParameter message)
        {
            var idRoleParameter = idRole.HasValue ?
                new ObjectParameter("IdRole", idRole) :
                new ObjectParameter("IdRole", typeof(int));
    
            var firstNameParameter = firstName != null ?
                new ObjectParameter("FirstName", firstName) :
                new ObjectParameter("FirstName", typeof(string));
    
            var lastNameParameter = lastName != null ?
                new ObjectParameter("LastName", lastName) :
                new ObjectParameter("LastName", typeof(string));
    
            var documentParameter = document != null ?
                new ObjectParameter("Document", document) :
                new ObjectParameter("Document", typeof(string));
    
            var phoneParameter = phone != null ?
                new ObjectParameter("Phone", phone) :
                new ObjectParameter("Phone", typeof(string));
    
            var addressParameter = address != null ?
                new ObjectParameter("Address", address) :
                new ObjectParameter("Address", typeof(string));
    
            var departmentParameter = department != null ?
                new ObjectParameter("Department", department) :
                new ObjectParameter("Department", typeof(string));
    
            var cityParameter = city != null ?
                new ObjectParameter("City", city) :
                new ObjectParameter("City", typeof(string));
    
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            var arlParameter = arl != null ?
                new ObjectParameter("Arl", arl) :
                new ObjectParameter("Arl", typeof(string));
    
            var epsParameter = eps != null ?
                new ObjectParameter("Eps", eps) :
                new ObjectParameter("Eps", typeof(string));
    
            var positionParameter = position != null ?
                new ObjectParameter("Position", position) :
                new ObjectParameter("Position", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_UserRegisteredEmployees", idRoleParameter, firstNameParameter, lastNameParameter, documentParameter, phoneParameter, addressParameter, departmentParameter, cityParameter, emailParameter, passwordParameter, arlParameter, epsParameter, positionParameter, registered, message);
        }
    
        public virtual ObjectResult<Nullable<int>> sp_VerifyUser(string email, string password)
        {
            var emailParameter = email != null ?
                new ObjectParameter("Email", email) :
                new ObjectParameter("Email", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_VerifyUser", emailParameter, passwordParameter);
        }
    }
}
