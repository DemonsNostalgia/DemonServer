SET @godship_column_exists = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_user'
    AND column_name = 'godship'
);
SET @godship_ddl = IF(
  @godship_column_exists = 0,
  'ALTER TABLE `cq_user` ADD COLUMN `godship` tinyint unsigned NOT NULL DEFAULT ''0'' AFTER `godlevel`',
  'SELECT 1'
);
PREPARE godship_statement FROM @godship_ddl;
EXECUTE godship_statement;
DEALLOCATE PREPARE godship_statement;

SET @godtype_column_exists = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_user'
    AND column_name = 'godtype'
);
SET @godtype_ddl = IF(
  @godtype_column_exists = 0,
  'ALTER TABLE `cq_user` ADD COLUMN `godtype` tinyint unsigned NOT NULL DEFAULT ''0'' AFTER `godship`',
  'SELECT 1'
);
PREPARE godtype_statement FROM @godtype_ddl;
EXECUTE godtype_statement;
DEALLOCATE PREPARE godtype_statement;
