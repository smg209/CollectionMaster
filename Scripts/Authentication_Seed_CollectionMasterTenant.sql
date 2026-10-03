-- =============================================================================
-- Authentication - development seed for CollectionMaster
--
-- Run against the AUTHENTICATION database, not CollectionMaster.
--
-- FSAuthentication only signs a user in to an application when that user is mapped to a
-- Tenant row whose AppName is that application's name. This script gives one EXISTING
-- login a CollectionMaster tenant:
--   1. Tenant              AppName = 'CollectionMaster'
--   2. UserAuth2TenantMap  that tenant <-> the login named in @Email
--
-- It creates no user and touches no password. It is safe to run more than once: if the
-- login already has a CollectionMaster tenant, nothing is inserted.
--
-- In CollectionMaster every collector is their own tenant, so this is one tenant for one
-- person - not a shared company. Real sign-ups will create these two rows from the app's
-- onboarding code; this script only exists so development can start before that is built.
--
-- MFA is left off for this tenant: with MFA_Required = 1 sign-in stops to email a code,
-- and CollectionMaster has no email provider configured yet.
--
-- STATUS:
--   [x] Applied 2026-10-02 - localhost\SQL2025 (steven-evoc, dev)
-- =============================================================================

USE Authentication;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Email      NVARCHAR(255) = N'lgrover@tworld.com';   -- the existing login to map
DECLARE @AppName    NVARCHAR(100) = N'CollectionMaster';     -- must equal CMConstants.AppName

DECLARE @UserAuthOid BIGINT = (SELECT Oid FROM UserAuth WHERE Email = @Email);

IF @UserAuthOid IS NULL
BEGIN
    RAISERROR('No UserAuth row has the email %s. Nothing was changed.', 16, 1, @Email);
    RETURN;
END

DECLARE @TenantOid BIGINT = (
    SELECT TOP (1) t.Oid
    FROM Tenant t
    JOIN UserAuth2TenantMap m ON m.TenantOid = t.Oid
    WHERE t.AppName = @AppName
      AND m.UserAuthOid = @UserAuthOid
    ORDER BY t.Oid
);

IF @TenantOid IS NOT NULL
BEGIN
    PRINT 'This login already has a CollectionMaster tenant. Nothing was changed.';
END
ELSE
BEGIN
    BEGIN TRANSACTION;

    INSERT INTO Tenant (AppName, CompanyName, IsActive, NumberOfFailedAttemptsAllowed, MFA_Required, MFA_RequiredOnRegistration, MFA_Enabled)
    SELECT @AppName, LTRIM(RTRIM(u.FirstName + N' ' + u.LastName)), 1, 5, 0, 0, 0
    FROM UserAuth u
    WHERE u.Oid = @UserAuthOid;

    SET @TenantOid = SCOPE_IDENTITY();

    INSERT INTO UserAuth2TenantMap (TenantOid, UserAuthOid)
    VALUES (@TenantOid, @UserAuthOid);

    COMMIT TRANSACTION;

    PRINT 'Created the CollectionMaster tenant and mapped the login to it.';
END

-- What sign-in will find for this login and application.
SELECT t.Oid AS TenantOid, t.AppName, t.CompanyName, t.MFA_Required, m.Oid AS MapOid, u.Oid AS UserAuthOid, u.Email
FROM Tenant t
JOIN UserAuth2TenantMap m ON m.TenantOid = t.Oid
JOIN UserAuth u ON u.Oid = m.UserAuthOid
WHERE t.AppName = @AppName
  AND u.Oid = @UserAuthOid;
GO
