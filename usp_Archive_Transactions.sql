
CREATE PROCEDURE [dbo].[usp_Archive_Transactions]

AS
BEGIN
	/********************************************************************************************************
	Filename	: usp_Archive_Transactions
	Description	: Archive the transactions which has reached the threshold limit
	********************************************************************************************************/

	SET NOCOUNT ON
	
	BEGIN TRY
	
		DECLARE @Updatedtransaction TABLE
		(
			TransactionID BIGINT
		)
	
		INSERT INTO
			@Updatedtransaction
		SELECT T.ID FROM 
			Transactions T WITH(NOLOCK)
		WHERE
			ISNULL(T.IsArchived, 0) != 1
			AND T.PrefillXmlID IS NOT NULL
			AND dbo.udf_GetArchiveThresholdLimit(T.ID, UpdateDate, T.lookupStatusId, T.DistributorID, T.IsShellTransaction) <= 0

		UPDATE T WITH(ROWLOCK)
		SET 
			T.IsArchived = 1
			, T.ArchivedDate = GETDATE()
		FROM 
			Transactions T 
		INNER JOIN 
			@Updatedtransaction tbl
				ON T.ID = tbl.TransactionID  

		SELECT * FROM @Updatedtransaction 
			
	END TRY
	BEGIN CATCH 
	END CATCH

	SET NOCOUNT OFF
END


