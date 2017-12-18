BEGIN TRY
    BEGIN TRANSACTION;
		INSERT INTO Programs(ID, Name, Description, ParentID, ProgramCode)
		VALUES(1001, 'STIs and OIs', 'STIs and OIs', 11, NULL)
    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION;

			BEGIN TRANSACTION;
			SET IDENTITY_INSERT Programs ON
			INSERT INTO Programs(ID, Name, Description, ParentID, ProgramCode)
			VALUES(1001, 'STIs and OIs', 'STIs and OIs', 11, NULL)
			SET IDENTITY_INSERT Programs OFF
		COMMIT TRANSACTION;
		END
  END CATCH