﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AbcPersistent.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class abcbdEntities : DbContext
    {
        public abcbdEntities()
            : base("name=abcbdEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AckSet> AckSet { get; set; }
        public virtual DbSet<ActionsSet> ActionsSet { get; set; }
        public virtual DbSet<AnalysticsAreaSet> AnalysticsAreaSet { get; set; }
        public virtual DbSet<AnalystSet> AnalystSet { get; set; }
        public virtual DbSet<AnalyticsMethodSet> AnalyticsMethodSet { get; set; }
        public virtual DbSet<BaseParamFamilySet> BaseParamFamilySet { get; set; }
        public virtual DbSet<BaseParamSet> BaseParamSet { get; set; }
        public virtual DbSet<ContainerSet> ContainerSet { get; set; }
        public virtual DbSet<EnterpriseSet> EnterpriseSet { get; set; }
        public virtual DbSet<MarketSet> MarketSet { get; set; }
        public virtual DbSet<MatrixChangesSet> MatrixChangesSet { get; set; }
        public virtual DbSet<MatrixSet> MatrixSet { get; set; }
        public virtual DbSet<MethodSet> MethodSet { get; set; }
        public virtual DbSet<NormSet> NormSet { get; set; }
        public virtual DbSet<OfficeSet> OfficeSet { get; set; }
        public virtual DbSet<PackageSet> PackageSet { get; set; }
        public virtual DbSet<ParameterSet> ParameterSet { get; set; }
        public virtual DbSet<PreserverSet> PreserverSet { get; set; }
        public virtual DbSet<Prices> Prices { get; set; }
        public virtual DbSet<RegionSet> RegionSet { get; set; }
        public virtual DbSet<ResidueSet> ResidueSet { get; set; }
        public virtual DbSet<RightSet> RightSet { get; set; }
        public virtual DbSet<RoleSet> RoleSet { get; set; }
        public virtual DbSet<SessionLogSet> SessionLogSet { get; set; }
        public virtual DbSet<UnitSet> UnitSet { get; set; }
        public virtual DbSet<UserSet> UserSet { get; set; }
        public virtual DbSet<GroupSet> GroupSet { get; set; }
    }
}
