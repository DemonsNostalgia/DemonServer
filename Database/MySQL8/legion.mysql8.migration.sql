-- Run once against an existing MySQL 8 soul database before deploying the
-- matching DBServer and MapServer binaries.

ALTER TABLE `cq_legion`
  ADD UNIQUE KEY `uq_cq_legion_name` (`name`);

ALTER TABLE `cq_legion_members`
  MODIFY `legion_id` int unsigned NOT NULL,
  ADD COLUMN `player_id` int NOT NULL DEFAULT '0' AFTER `legion_id`,
  MODIFY `members_name` varchar(32) NOT NULL,
  MODIFY `money` bigint NOT NULL DEFAULT '0',
  ADD COLUMN `emoney` bigint NOT NULL DEFAULT '0' AFTER `money`,
  MODIFY `rank` smallint NOT NULL DEFAULT '200';

UPDATE `cq_legion_members` AS member
INNER JOIN `cq_user` AS player
  ON player.`name` = member.`members_name`
SET member.`player_id` = player.`id`
WHERE member.`player_id` = 0;

INSERT INTO `cq_legion_members`
  (`legion_id`,`player_id`,`members_name`,`money`,`emoney`,`rank`)
SELECT legion.`id`,
       legion.`leader_id`,
       legion.`leader_name`,
       legion.`money`,
       0,
       1000
FROM `cq_legion` AS legion
LEFT JOIN `cq_legion_members` AS member
  ON member.`legion_id` = legion.`id`
 AND member.`player_id` = legion.`leader_id`
WHERE member.`id` IS NULL;

ALTER TABLE `cq_legion_members`
  ALTER COLUMN `player_id` DROP DEFAULT,
  ADD UNIQUE KEY `uq_cq_legion_member_player` (`player_id`),
  ADD UNIQUE KEY `uq_cq_legion_member_name`
    (`legion_id`,`members_name`),
  ADD KEY `ix_cq_legion_member_legion` (`legion_id`);
