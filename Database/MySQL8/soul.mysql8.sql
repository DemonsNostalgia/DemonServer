
/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `id` int NOT NULL AUTO_INCREMENT,
  `account` varchar(32) DEFAULT NULL,
  `password` varchar(32) DEFAULT NULL,
  `vip` tinyint DEFAULT NULL,
  `serverindex` int DEFAULT '-1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES (1,'fucknd','123456',1,-1),(2,'123456','113',1,-1),(3,'ceshi','123',1,-1),(4,'1','1',1,-1),(5,'2','2',1,-1);
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_eudemon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_eudemon` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `itemid` int DEFAULT '0',
  `ownerid` int DEFAULT '0',
  `name` varchar(255) DEFAULT '',
  `phyatk_grow_rate` int DEFAULT '0',
  `phyatk_grow_rate_max` int DEFAULT '0',
  `magicatk_grow_rate` int DEFAULT '0',
  `magicatk_grow_rate_max` int DEFAULT '0',
  `life_grow_rate` int DEFAULT '0',
  `defense_grow_rate` int DEFAULT '0',
  `magicdef_grow_rate` int DEFAULT '0',
  `life` int DEFAULT '0',
  `atk_min` int DEFAULT '0',
  `atk_max` int DEFAULT '0',
  `magicatk_min` int DEFAULT '0',
  `magicatk_max` int DEFAULT '0',
  `defense` int DEFAULT '0',
  `magicdef` int DEFAULT '0',
  `luck` int DEFAULT '0',
  `intimacy` int DEFAULT '0',
  `level` smallint DEFAULT '1',
  `card` int DEFAULT '0',
  `exp` int DEFAULT '0',
  `quality` int DEFAULT '0',
  `wuxing` int DEFAULT '0',
  `recall_count` int DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_eudemon` WRITE;
/*!40000 ALTER TABLE `cq_eudemon` DISABLE KEYS */;
INSERT INTO `cq_eudemon` VALUES (1,7,1,'Mage~AtkDef',2,5,4,23,34,7,9,70,28,19,60,56,53,88,4,150,1,0,0,0,2,0),(2,8,1,'Mage~AtkDef',2,4,18,27,5,22,2,17,23,11,79,57,28,87,86,150,1,0,0,0,1,0),(3,26,2,'Warrior~AtkDef',7,38,0,0,31,16,1,78,22,24,0,39,77,33,23,150,51,366885,689,1000,2,0),(4,17,2,'Warrior~AtkDef',5,30,0,0,34,3,10,50,73,54,0,20,82,47,87,150,79,347245051,120450,1000,3,0),(5,32,2,'Universal~O',6,20,3,14,9,16,1,270,52,10,22,84,52,38,9,150,80,0,84,600,3,0),(6,34,2,'XO Eudemon',2,22,4,13,21,12,8,232,86,14,64,43,89,27,64,150,80,0,0,1200,1,0),(7,55,2,'XO Eudemon',4,23,7,15,6,16,5,242,93,80,10,70,14,62,44,150,80,0,0,1900,1,0),(8,55,2,'XO Eudemon',4,23,7,15,6,16,5,242,93,80,10,70,14,62,44,150,80,0,0,1900,1,0),(9,64,2,'Saint~XO',13,20,4,15,49,6,1,104,26,59,79,29,61,40,28,150,55,0,339,900,2,0),(10,67,2,'Saint~XO',11,24,10,15,39,11,5,135,10,69,74,83,42,29,59,150,80,0,490137,2500,3,0),(11,69,2,'XO Eudemon',16,20,6,16,31,6,5,19,86,75,20,95,48,60,29,150,80,0,1433,2500,4,0),(12,69,2,'XO Eudemon',16,20,6,16,31,6,5,19,86,75,20,95,48,60,29,150,80,0,1476,2500,4,0),(13,69,2,'XO Eudemon',16,20,6,16,31,6,5,19,86,75,20,95,48,60,29,150,80,0,1476,2500,4,0),(14,69,2,'XO Eudemon',16,20,6,16,31,6,5,19,86,75,20,95,48,60,29,150,80,0,1476,2500,4,0),(15,77,3,'Mage~AtkDef',1,4,15,25,27,16,9,207,18,20,74,40,10,46,9,150,1,0,0,0,1,0),(16,78,3,'Mage~AtkDef',2,3,10,26,52,21,3,106,29,11,42,15,72,58,68,150,1,0,0,0,2,0),(17,80,4,'Mage~AtkDef',1,4,4,25,41,12,11,60,17,26,45,45,19,30,33,150,1,0,0,0,4,0),(18,81,4,'Mage~AtkDef',2,5,13,20,36,18,8,126,14,25,81,21,88,49,86,150,1,0,0,0,2,0),(19,83,5,'Mage~AtkDef',2,5,14,24,29,21,6,172,14,27,36,39,52,33,15,150,1,0,0,0,3,0),(20,84,5,'Mage~AtkDef',1,4,20,22,10,4,8,231,28,24,69,47,45,41,76,150,1,0,0,0,3,0),(21,133,6,'Dragon Soul Guard - Seifer',22,34,0,0,49,7,9,211,37,50,0,0,41,52,97,150,1,0,0,0,1,0),(22,134,6,'Dragon Soul Guard - Seifer',18,35,0,0,25,18,3,40,53,56,0,0,41,41,41,150,1,0,0,0,1,0),(23,138,6,'Universal~O',16,19,8,16,33,8,10,152,96,24,26,60,38,46,3,150,1,0,0,0,3,0),(24,138,6,'Universal~O',16,19,8,16,33,8,10,152,96,24,26,60,38,46,3,150,1,0,0,0,3,0),(25,146,2,'Warrior~Lulu',16,37,0,0,63,7,12,185,57,17,0,0,75,72,52,150,51,0,0,307,3,0),(26,148,2,'Saint~XO',1,19,8,15,61,13,14,260,75,65,33,87,24,61,56,150,51,0,0,664,4,0),(27,150,7,'Dragon Soul Guard - Seifer',17,38,0,0,11,6,6,154,24,33,0,0,85,52,69,150,1,0,0,0,1,0),(28,151,7,'Dragon Soul Guard - Seifer',2,31,0,0,20,19,5,141,95,26,0,0,60,47,27,150,1,0,0,0,2,0),(29,150,7,'Dragon Soul Guard - Seifer',17,38,0,0,11,6,6,154,24,33,0,0,85,52,69,150,1,0,0,0,1,0),(30,151,7,'Dragon Soul Guard - Seifer',2,31,0,0,20,19,5,141,95,26,0,0,60,47,27,150,1,0,0,0,2,0),(31,165,7,'Nova the Lamb',7,27,0,0,16,16,12,577,85,69,0,0,73,65,45,150,51,0,0,147,1,0),(32,169,7,'Star Scar Crystal Mare Luna',3,39,7,30,13,16,14,128,28,98,36,78,41,82,80,150,1,0,0,1163,2,10),(33,171,7,'Karavos',20,34,0,0,15,1,8,227,20,84,0,0,17,52,29,150,5,0,37,0,3,0),(34,176,8,'Dark Soul Guard - Oren',21,33,0,0,36,2,7,93,65,12,0,0,80,54,5,150,1,0,0,0,3,0),(35,177,8,'Dark Soul Guard - Oren',11,32,0,0,53,7,9,174,67,51,0,0,49,39,75,150,1,0,0,0,2,0),(36,444,7,'Universal O',2,23,12,13,23,17,15,38,16,54,51,53,37,42,89,150,80,0,0,600,2,0),(37,445,7,'Universal O',7,21,12,13,47,15,14,240,16,17,66,67,69,72,27,150,80,0,0,600,2,0),(38,446,7,'Universal O',7,23,4,16,35,3,9,180,37,88,59,76,21,72,7,150,80,0,0,600,2,0),(39,419,7,'XO Eudemon',6,24,6,16,22,17,11,14,16,88,21,97,60,89,69,150,80,0,0,1200,3,0),(40,420,7,'XO Eudemon',8,20,3,17,2,7,14,156,42,93,69,69,10,40,9,150,80,0,0,1200,2,0),(41,421,7,'XO Eudemon',16,22,3,13,17,6,5,82,59,92,75,95,83,80,40,150,80,0,0,1200,3,0),(42,422,7,'XO Eudemon',10,19,6,17,8,15,2,229,31,71,35,90,36,53,25,150,80,0,0,1200,1,0),(43,423,7,'XO Eudemon',1,20,9,13,16,3,3,36,20,88,13,80,14,95,40,150,80,0,0,1200,2,0),(44,424,7,'XO Eudemon',16,20,2,16,62,10,10,149,27,17,25,44,81,35,57,150,80,0,0,1200,4,0),(45,425,7,'XO Eudemon',1,18,6,15,6,9,3,234,12,63,72,13,89,89,22,150,80,0,0,1200,3,0),(46,426,7,'XO Eudemon',12,21,4,17,37,2,1,46,85,24,55,33,77,85,57,150,80,0,0,1200,4,0),(47,427,7,'XO Eudemon',9,19,4,15,59,1,13,245,41,44,39,49,69,51,99,150,80,0,0,1200,3,0),(48,428,7,'XO Eudemon',9,20,6,16,9,4,7,64,97,97,65,79,47,92,93,150,80,0,0,1200,3,0),(49,429,7,'XO Eudemon',15,18,2,16,27,4,14,200,85,10,57,33,66,83,62,150,80,0,0,1200,2,0),(50,430,7,'XO Eudemon',11,23,3,13,54,12,10,65,91,31,40,55,65,21,87,150,80,0,0,1200,3,0),(51,431,7,'XO Eudemon',14,21,7,17,51,11,3,96,28,15,11,34,63,15,9,150,80,0,0,1200,2,0),(52,432,7,'XO Eudemon',16,22,11,15,14,6,6,289,29,36,42,73,63,90,6,150,80,0,0,1200,3,0),(53,433,7,'XO Eudemon',14,22,8,15,58,4,15,228,21,48,58,17,25,38,50,150,80,0,0,1200,1,0),(54,444,7,'Universal O',2,23,12,13,23,17,15,38,16,54,51,53,37,42,89,150,80,0,0,600,2,0),(55,445,7,'Universal O',7,21,12,13,47,15,14,240,16,17,66,67,69,72,27,150,80,0,0,600,2,0),(56,446,7,'Universal O',7,23,4,16,35,3,9,180,37,88,59,76,21,72,7,150,80,0,0,600,2,0),(57,419,7,'XO Eudemon',6,24,6,16,22,17,11,14,16,88,21,97,60,89,69,150,80,0,0,1200,3,0),(58,420,7,'XO Eudemon',8,20,3,17,2,7,14,156,42,93,69,69,10,40,9,150,80,0,0,1200,2,0),(59,421,7,'XO Eudemon',16,22,3,13,17,6,5,82,59,92,75,95,83,80,40,150,80,0,0,1200,3,0),(60,422,7,'XO Eudemon',10,19,6,17,8,15,2,229,31,71,35,90,36,53,25,150,80,0,0,1200,1,0),(61,423,7,'XO Eudemon',1,20,9,13,16,3,3,36,20,88,13,80,14,95,40,150,80,0,0,1200,2,0),(62,424,7,'XO Eudemon',16,20,2,16,62,10,10,149,27,17,25,44,81,35,57,150,80,0,0,1200,4,0),(63,425,7,'XO Eudemon',1,18,6,15,6,9,3,234,12,63,72,13,89,89,22,150,80,0,0,1200,3,0),(64,426,7,'XO Eudemon',12,21,4,17,37,2,1,46,85,24,55,33,77,85,57,150,80,0,0,1200,4,0),(65,427,7,'XO Eudemon',9,19,4,15,59,1,13,245,41,44,39,49,69,51,99,150,80,0,0,1200,3,0),(66,428,7,'XO Eudemon',9,20,6,16,9,4,7,64,97,97,65,79,47,92,93,150,80,0,0,1200,3,0),(67,429,7,'XO Eudemon',15,18,2,16,27,4,14,200,85,10,57,33,66,83,62,150,80,0,0,1200,2,0),(68,430,7,'XO Eudemon',11,23,3,13,54,12,10,65,91,31,40,55,65,21,87,150,80,0,0,1200,3,0),(69,431,7,'XO Eudemon',14,21,7,17,51,11,3,96,28,15,11,34,63,15,9,150,80,0,0,1200,2,0),(70,432,7,'XO Eudemon',16,22,11,15,14,6,6,289,29,36,42,73,63,90,6,150,80,0,1970,1200,3,0),(71,433,7,'XO Eudemon',14,22,8,15,58,4,15,228,21,48,58,17,25,38,50,150,80,0,1970,1200,1,0);
/*!40000 ALTER TABLE `cq_eudemon` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_eudemon_magic`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_eudemon_magic` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `ownerid` int DEFAULT NULL,
  `magicid` int DEFAULT NULL,
  `level` tinyint DEFAULT NULL,
  `exp` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_eudemon_magic` WRITE;
/*!40000 ALTER TABLE `cq_eudemon_magic` DISABLE KEYS */;
/*!40000 ALTER TABLE `cq_eudemon_magic` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_friend`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_friend` (
  `id` int NOT NULL AUTO_INCREMENT,
  `userid` int NOT NULL,
  `friendtype` tinyint unsigned NOT NULL DEFAULT '15',
  `friendid` int NOT NULL,
  `friendname` varchar(35) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_cq_friend_relation` (`userid`,`friendtype`,`friendid`),
  KEY `ix_cq_friend_reverse` (`friendid`,`friendtype`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_friend` WRITE;
/*!40000 ALTER TABLE `cq_friend` DISABLE KEYS */;
/*!40000 ALTER TABLE `cq_friend` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_item` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `playerid` int NOT NULL,
  `itemid` int NOT NULL,
  `postion` tinyint unsigned DEFAULT '0',
  `stronglv` tinyint unsigned DEFAULT '0',
  `gemcount` tinyint DEFAULT '0',
  `gem1` tinyint unsigned DEFAULT '0',
  `gem2` tinyint unsigned DEFAULT '0',
  `forgename` varchar(32) DEFAULT '',
  `amount` int DEFAULT '0',
  `war_ghost_exp` int DEFAULT '0',
  `di_attack` tinyint unsigned DEFAULT '0',
  `shui_attack` tinyint unsigned DEFAULT '0',
  `huo_attack` tinyint unsigned DEFAULT '0',
  `feng_attack` tinyint unsigned DEFAULT '0',
  `property` int DEFAULT '0',
  `gem3` tinyint unsigned DEFAULT '0',
  `god_exp` int DEFAULT '0',
  `god_strong` int DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=454 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_item` WRITE;
/*!40000 ALTER TABLE `cq_item` DISABLE KEYS */;
INSERT INTO `cq_item` VALUES (1,1,115041,1,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(2,1,125041,2,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(3,1,135041,3,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(4,1,440101,4,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(5,1,145041,7,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(6,1,165031,8,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(7,1,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(8,1,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(9,2,111041,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(10,2,121041,2,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(11,2,131041,3,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(13,2,141041,7,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(14,2,161091,8,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(15,2,420103,50,9,0,255,255,'',10000,0,9,9,9,9,0,0,0,0),(17,2,1071022,53,0,0,0,0,'Warrior~AtkDef',1,0,0,0,0,0,0,0,0,0),(18,2,1038170,50,0,0,54,0,'',1,0,0,0,0,0,0,0,0,0),(19,2,1038200,50,0,0,50,0,'',1,0,0,0,0,0,0,0,0,0),(20,2,1038230,50,0,0,9,0,'',1,0,0,0,0,0,0,0,0,0),(21,2,1038260,50,0,0,63,0,'',1,0,0,0,0,0,0,0,0,0),(22,2,1038290,50,0,0,52,0,'',1,0,0,0,0,0,0,0,0,0),(26,2,1071022,53,0,0,0,0,'Warrior~AtkDef',1,0,0,0,0,0,0,0,0,0),(36,2,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(37,2,813003,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(39,2,813003,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(40,2,813004,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(41,2,813005,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(46,2,743494,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(48,2,1021070,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(49,2,1021080,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(64,2,1071961,53,0,0,0,0,'Saint~XO',1,0,0,0,0,0,0,0,0,0),(67,2,1071962,53,0,0,0,0,'Saint~XO',1,0,0,0,0,0,0,0,0,0),(69,2,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(70,2,415070,49,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(71,3,115041,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(72,3,125041,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(73,3,135041,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(74,3,440101,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(75,3,145041,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(76,3,165031,50,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(77,3,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(78,3,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(79,4,729000,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(80,4,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(81,4,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(82,5,729000,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(83,5,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(84,5,1071220,53,0,0,0,0,'Mage~AtkDef',1,0,0,0,0,0,0,0,0,0),(85,5,440000,4,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(86,5,135300,3,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(87,2,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(88,2,745960,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(89,2,779001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(90,2,729032,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(91,2,729032,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(94,2,729032,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(95,2,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(97,2,1021080,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(98,2,728077,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(99,2,728077,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(100,2,728075,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(101,2,748385,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(102,2,729032,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(103,2,729032,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(105,2,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(108,2,111041,1,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(120,2,410101,4,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(132,6,729000,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(133,6,1072560,53,0,0,0,0,'Dragon Soul Guard - Seifer',1,0,0,0,0,0,0,0,0,0),(134,6,1072560,53,0,0,0,0,'Dragon Soul Guard - Seifer',1,0,0,0,0,0,0,0,0,0),(135,6,480000,4,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(136,6,132300,3,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(138,6,1071990,53,0,0,0,0,'Universal~O',1,0,0,0,0,0,0,0,0,0),(144,2,724024,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(146,2,1071421,53,0,0,0,0,'Warrior~Lulu',1,0,0,0,0,0,0,0,0,0),(148,2,1071961,53,0,0,0,0,'Saint~XO',1,0,0,0,0,0,0,0,0,0),(150,7,1072560,53,0,0,0,0,'Dragon Soul Guard - Seifer',1,0,0,0,0,0,0,0,0,0),(151,7,1072560,53,0,0,0,0,'Dragon Soul Guard - Seifer',1,0,0,0,0,0,0,0,0,0),(152,7,480001,100,0,0,0,0,'biggie[PM]',10000,0,0,0,0,0,0,0,0,0),(153,7,132300,100,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(158,7,142041,7,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(159,7,162031,8,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(160,7,112042,1,0,0,0,0,'biggie[PM]',1,0,0,0,0,0,0,0,0,0),(161,7,480104,4,0,0,0,0,'biggie[PM]',1,0,0,0,0,0,0,0,0,0),(162,7,132044,3,0,0,0,0,'biggie[PM]',1,0,0,0,0,0,0,0,0,0),(163,7,122042,2,0,0,0,0,'biggie[PM]',1,0,0,0,0,0,0,0,0,0),(165,7,1073971,53,0,0,0,0,'Nova the Lamb',1,0,0,0,0,0,0,0,0,0),(166,7,192180,44,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(167,7,191300,44,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(169,7,1072520,53,0,0,0,0,'Star Scar Crystal Mare Luna',1,0,0,0,0,0,0,0,0,0),(171,7,1072060,53,0,0,0,0,'Karavos',1,0,0,0,0,0,0,0,0,0),(172,7,749390,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(173,7,749391,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(174,7,749392,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(175,8,729000,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(176,8,1072210,53,0,0,0,0,'Dark Soul Guard - Oren',1,0,0,0,0,0,0,0,0,0),(177,8,1072210,53,0,0,0,0,'Dark Soul Guard - Oren',1,0,0,0,0,0,0,0,0,0),(178,8,450000,4,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(179,8,137300,3,0,0,0,0,'',10000,0,0,0,0,0,0,0,0,0),(180,7,748995,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(181,7,810010,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(188,7,1025167,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(189,7,744748,100,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(382,7,191660,44,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(383,7,191040,12,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(419,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(420,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(421,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(422,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(423,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(424,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(425,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(426,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(427,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(428,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(429,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(430,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(431,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(432,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(433,7,1071982,53,0,0,0,0,'XO Eudemon',1,0,0,0,0,0,0,0,0,0),(435,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(438,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(439,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(440,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(441,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(442,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(443,7,813001,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(444,7,1071992,53,0,0,0,0,'Universal O',1,0,0,0,0,0,0,0,0,0),(445,7,1071992,53,0,0,0,0,'Universal O',1,0,0,0,0,0,0,0,0,0),(446,7,1071992,53,0,0,0,0,'Universal O',1,0,0,0,0,0,0,0,0,0),(447,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(448,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(449,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(450,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(451,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0),(452,7,1040039,50,0,0,0,0,'',1,0,0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `cq_item` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_legion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_legion` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(32) DEFAULT NULL,
  `member_title` tinyint DEFAULT NULL,
  `leader_id` int DEFAULT NULL,
  `leader_name` varchar(32) DEFAULT NULL,
  `money` bigint DEFAULT NULL,
  `notice` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_cq_legion_name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_legion` WRITE;
/*!40000 ALTER TABLE `cq_legion` DISABLE KEYS */;
INSERT INTO `cq_legion` VALUES (1,'test',0,7,'biggie[PM]',250000,'Announcement');
/*!40000 ALTER TABLE `cq_legion` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_legion_members`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_legion_members` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `legion_id` int unsigned NOT NULL,
  `player_id` int NOT NULL,
  `members_name` varchar(32) NOT NULL,
  `money` bigint NOT NULL DEFAULT '0',
  `emoney` bigint NOT NULL DEFAULT '0',
  `rank` smallint NOT NULL DEFAULT '200',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_cq_legion_member_player` (`player_id`),
  UNIQUE KEY `uq_cq_legion_member_name` (`legion_id`,`members_name`),
  KEY `ix_cq_legion_member_legion` (`legion_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_legion_members` WRITE;
/*!40000 ALTER TABLE `cq_legion_members` DISABLE KEYS */;
INSERT INTO `cq_legion_members`
  (`legion_id`,`player_id`,`members_name`,`money`,`emoney`,`rank`)
VALUES (1,7,'biggie[PM]',250000,0,1000);
/*!40000 ALTER TABLE `cq_legion_members` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_login_ticket`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_login_ticket` (
  `account` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `account_id` int unsigned NOT NULL,
  `server_name` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `expires_at` datetime(6) NOT NULL,
  PRIMARY KEY (`account`),
  KEY `ix_cq_login_ticket_expires` (`expires_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_login_ticket` WRITE;
/*!40000 ALTER TABLE `cq_login_ticket` DISABLE KEYS */;
INSERT INTO `cq_login_ticket` VALUES ('1',4,'soul','2026-07-29 20:02:28.003742');
/*!40000 ALTER TABLE `cq_login_ticket` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_magic`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_magic` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `ownerid` int DEFAULT '0',
  `magicid` int DEFAULT '0',
  `level` tinyint DEFAULT NULL COMMENT '0',
  `exp` int DEFAULT NULL COMMENT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=55 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_magic` WRITE;
/*!40000 ALTER TABLE `cq_magic` DISABLE KEYS */;
INSERT INTO `cq_magic` VALUES (1,1,3000,0,0),(2,1,3001,0,0),(3,1,3002,0,0),(4,1,3003,0,0),(5,1,3004,0,0),(6,1,3005,0,0),(7,1,3006,0,0),(8,1,3009,0,0),(9,1,3011,0,0),(10,1,3010,0,0),(11,1,5300,0,0),(12,1,5301,0,0),(13,1,5302,0,0),(14,1,5309,0,0),(15,1,5310,0,0),(16,2,1007,0,0),(17,2,1009,0,0),(18,2,1007,0,3),(19,2,1009,0,0),(20,2,1007,0,31),(21,2,1009,0,6),(22,3,3000,0,0),(23,3,3001,0,0),(24,3,3002,0,0),(25,3,3003,0,0),(26,3,3004,0,0),(27,3,3005,0,0),(28,3,3006,0,0),(29,3,3009,0,0),(30,3,3011,0,0),(31,3,3010,0,0),(32,3,5300,0,0),(33,3,5301,0,0),(34,3,5302,0,0),(35,3,5309,0,0),(36,3,5310,0,0),(37,2,1010,0,0),(38,2,1010,0,0),(39,2,1010,0,0),(40,2,1010,0,0),(41,2,1010,0,0),(42,2,1010,0,0),(43,7,5215,0,0),(44,7,5223,0,0),(45,7,5216,0,0),(46,7,5241,0,22),(47,7,5211,0,0),(48,7,5245,0,0),(49,7,5215,0,0),(50,7,5223,3,0),(51,7,5216,0,0),(52,7,5241,0,50),(53,7,5211,0,0),(54,7,5245,0,0);
/*!40000 ALTER TABLE `cq_magic` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_payrec`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_payrec` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `order` varchar(255) DEFAULT NULL,
  `account` varchar(255) DEFAULT NULL,
  `money` int DEFAULT NULL,
  `state` tinyint DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_payrec` WRITE;
/*!40000 ALTER TABLE `cq_payrec` DISABLE KEYS */;
/*!40000 ALTER TABLE `cq_payrec` ENABLE KEYS */;
UNLOCK TABLES;
DROP TABLE IF EXISTS `cq_family_attr`;
DROP TABLE IF EXISTS `cq_family`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_family` (
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
CREATE TABLE `cq_family_attr` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `cq_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cq_user` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `accountid` int unsigned NOT NULL,
  `name` varchar(32) DEFAULT NULL,
  `lookface` int DEFAULT '0',
  `hair` int DEFAULT '0',
  `level` tinyint unsigned DEFAULT '1',
  `exp` int DEFAULT '0',
  `life` int DEFAULT '0',
  `mana` int DEFAULT '0',
  `profession` tinyint DEFAULT '0',
  `pk` int DEFAULT '0',
  `gold` int unsigned DEFAULT '0',
  `gamegold` int unsigned DEFAULT '0',
  `stronggold` int DEFAULT '0',
  `mapid` int DEFAULT '0',
  `record_x` int DEFAULT '0',
  `record_y` int DEFAULT '0',
  `hotkey` varchar(255) DEFAULT '',
  `guanjue` bigint unsigned DEFAULT '0',
  `godlevel` tinyint unsigned DEFAULT '0',
  `godship` tinyint unsigned NOT NULL DEFAULT '0',
  `godtype` tinyint unsigned NOT NULL DEFAULT '0',
  `maxeudemon` tinyint DEFAULT '2',
  `wardrobe_hairs` varchar(2048) NOT NULL DEFAULT '',
  `wardrobe_avatars` varchar(2048) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `cq_user` WRITE;
/*!40000 ALTER TABLE `cq_user` DISABLE KEYS */;
INSERT INTO `cq_user` VALUES (1,1,'aaa[PM]',150001,0,1,0,50,60,10,0,0,0,0,1000,312,461,'',0,0,2,'',''),(2,2,'I am a warrior [PM]',170001,119,74,2710,3020,0,20,0,100000119,971133,0,1000,376,258,'2|0|1|0|2|1007|0,2|8|9|0|2|1010|0,',0,0,3,'119',''),(6,3,'I Am An''an [PM]',610001,0,1,123,100,0,70,0,0,972,0,1000,431,498,'',0,0,2,'',''),(7,4,'biggie[PM]',610001,0,210,3627,13700,0,70,0,101500000,99966889,0,1000,193,429,'2|8|9|0|2|5241|0,1|9|10|0|2|5211|0,1|10|11|0|2|5241|0,',0,0,3,'',''),(8,5,'test',510001,0,90,0,2050,60,50,0,0,1111111,0,1000,186,436,'',0,0,2,'','');
/*!40000 ALTER TABLE `cq_user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
