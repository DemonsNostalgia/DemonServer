-- MySQL 8 friend/enemy relationship schema upgrade.
-- The duplicate cleanup makes the unique relation key safe to add.
DELETE older
FROM `cq_friend` AS older
INNER JOIN `cq_friend` AS newer
  ON newer.`userid` = older.`userid`
 AND newer.`friendtype` = older.`friendtype`
 AND newer.`friendid` = older.`friendid`
 AND newer.`id` > older.`id`;

ALTER TABLE `cq_friend`
  MODIFY `userid` int NOT NULL,
  MODIFY `friendtype` tinyint unsigned NOT NULL DEFAULT '15',
  MODIFY `friendid` int NOT NULL,
  MODIFY `friendname` varchar(35) NOT NULL DEFAULT '';

SET @friend_unique_exists = (
  SELECT COUNT(*)
  FROM information_schema.statistics
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_friend'
    AND index_name = 'uq_cq_friend_relation'
);
SET @friend_unique_sql = IF(
  @friend_unique_exists = 0,
  'ALTER TABLE `cq_friend` ADD UNIQUE KEY `uq_cq_friend_relation` (`userid`,`friendtype`,`friendid`)',
  'SELECT 1'
);
PREPARE friend_unique_statement FROM @friend_unique_sql;
EXECUTE friend_unique_statement;
DEALLOCATE PREPARE friend_unique_statement;

SET @friend_reverse_exists = (
  SELECT COUNT(*)
  FROM information_schema.statistics
  WHERE table_schema = DATABASE()
    AND table_name = 'cq_friend'
    AND index_name = 'ix_cq_friend_reverse'
);
SET @friend_reverse_sql = IF(
  @friend_reverse_exists = 0,
  'ALTER TABLE `cq_friend` ADD KEY `ix_cq_friend_reverse` (`friendid`,`friendtype`)',
  'SELECT 1'
);
PREPARE friend_reverse_statement FROM @friend_reverse_sql;
EXECUTE friend_reverse_statement;
DEALLOCATE PREPARE friend_reverse_statement;
