namespace Ictshop.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ictshop.Models.ShopShoe>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }       
    }
}
