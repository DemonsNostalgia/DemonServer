SET @wardrobe_mount_position_type = (
  SELECT LOWER(column_type)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_item'
    AND column_name = 'postion'
);
SET @wardrobe_mount_position_ddl = IF(
  @wardrobe_mount_position_type = 'tinyint unsigned',
  'SELECT 1',
  'ALTER TABLE `cq_item` MODIFY COLUMN `postion` tinyint unsigned DEFAULT 0'
);
PREPARE wardrobe_mount_position_statement
  FROM @wardrobe_mount_position_ddl;
EXECUTE wardrobe_mount_position_statement;
DEALLOCATE PREPARE wardrobe_mount_position_statement;
