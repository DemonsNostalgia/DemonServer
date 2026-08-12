SET @wardrobe_avatar_column_exists = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_user'
    AND column_name = 'wardrobe_avatars'
);
SET @wardrobe_avatar_ddl = IF(
  @wardrobe_avatar_column_exists = 0,
  'ALTER TABLE `cq_user` ADD COLUMN `wardrobe_avatars` varchar(2048) NOT NULL DEFAULT '''' AFTER `wardrobe_hairs`',
  'SELECT 1'
);
PREPARE wardrobe_avatar_statement FROM @wardrobe_avatar_ddl;
EXECUTE wardrobe_avatar_statement;
DEALLOCATE PREPARE wardrobe_avatar_statement;
