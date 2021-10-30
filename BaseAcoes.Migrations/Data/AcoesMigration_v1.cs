using FluentMigrator;

namespace BaseAcoes.Migrations
{
    [Migration(1)]
    public class AcoesMigration_v1 : Migration
    {
        public override void Up()
        {
    		Create.Table("Acoes")
	    		.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
		    	.WithColumn("Codigo").AsAnsiString(10).NotNullable()
                .WithColumn("Valor").AsDecimal(12,2).NotNullable()
		    	.WithColumn("DataReferencia").AsDateTime().NotNullable()
		    	.WithColumn("HistLancamento").AsAnsiString(50).NotNullable()
                .WithColumn("CodCorretora").AsAnsiString(10).NotNullable()
		    	.WithColumn("NomeCorretora").AsAnsiString(60).NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Acoes");
        }
    }
}