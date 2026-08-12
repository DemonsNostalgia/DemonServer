-- MySQL 8 family-system schema. Safe to run repeatedly.
CREATE TABLE IF NOT EXISTS `cq_family` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `family_name` varchar(15) NOT NULL,
  `rank` tinyint unsigned NOT NULL DEFAULT '0',
  `leader_name` varchar(32) NOT NULL,
  `leader_id` int NOT NULL,
  `announce` varchar(127) NOT NULL DEFAULT '',
  `money` bigint unsigned NOT NULL DEFAULT '0',
  `repute` int unsigned NOT NULL DEFAULT '0',
  `amount` int unsigned NOT NULL DEFAULT '0',
  `enemy_family0_id` int unsigned NOT NULL DEFAULT '0',
  `enemy_family1_id` int unsigned NOT NULL DEFAULT '0',
  `enemy_family2_id` int unsigned NOT NULL DEFAULT '0',
  `enemy_family3_id` int unsigned NOT NULL DEFAULT '0',
  `enemy_family4_id` int unsigned NOT NULL DEFAULT '0',
  `ally_family0_id` int unsigned NOT NULL DEFAULT '0',
  `ally_family1_id` int unsigned NOT NULL DEFAULT '0',
  `ally_family2_id` int unsigned NOT NULL DEFAULT '0',
  `ally_family3_id` int unsigned NOT NULL DEFAULT '0',
  `ally_family4_id` int unsigned NOT NULL DEFAULT '0',
  `create_date` int unsigned NOT NULL DEFAULT '0',
  `create_name` varchar(32) NOT NULL DEFAULT '',
  `del_flag` tinyint unsigned NOT NULL DEFAULT '0',
  `star_tower` tinyint unsigned NOT NULL DEFAULT '0',
  `challenge_map` int unsigned NOT NULL DEFAULT '0',
  `family_map` int unsigned NOT NULL DEFAULT '0',
  `truce` int unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_cq_family_name` (`family_name`),
  UNIQUE KEY `uq_cq_family_leader` (`leader_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `cq_family_attr` (
  `id` int NOT NULL,
  `family_id` int unsigned NOT NULL,
  `rank` smallint unsigned NOT NULL DEFAULT '10',
  `proffer` int unsigned NOT NULL DEFAULT '0',
  `join_date` int unsigned NOT NULL DEFAULT '0',
  `auto_exercise` tinyint unsigned NOT NULL DEFAULT '0',
  `exp_date` int unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `ix_cq_family_attr_family` (`family_id`),
  CONSTRAINT `fk_cq_family_attr_family` FOREIGN KEY (`family_id`)
    REFERENCES `cq_family` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
