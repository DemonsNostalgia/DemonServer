SET @wardrobe_hair_column_exists = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_user'
    AND column_name = 'wardrobe_hairs'
);
SET @wardrobe_hair_ddl = IF(
  @wardrobe_hair_column_exists = 0,
  'ALTER TABLE `cq_user` ADD COLUMN `wardrobe_hairs` varchar(2048) NOT NULL DEFAULT '''' AFTER `maxeudemon`',
  'SELECT 1'
);
PREPARE wardrobe_hair_statement FROM @wardrobe_hair_ddl;
EXECUTE wardrobe_hair_statement;
DEALLOCATE PREPARE wardrobe_hair_statement;

UPDATE `cq_user`
SET `wardrobe_hairs` = CAST(`hair` AS CHAR)
WHERE `hair` > 0
  AND (`wardrobe_hairs` IS NULL OR `wardrobe_hairs` = '');
