CREATE DATABASE  IF NOT EXISTS `raa` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `raa`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: raa
-- ------------------------------------------------------
-- Server version	5.6.14-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `content`
--

DROP TABLE IF EXISTS `content`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `content` (
  `contentid` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `objecturi` varchar(100) NOT NULL,
  `createdate` date DEFAULT NULL,
  `username` text,
  `applicationid` int(11) DEFAULT NULL,
  `tag` text,
  `coordinate` text,
  `comment` text,
  `relationtype` varchar(100) DEFAULT NULL,
  `relatedto` varchar(100) DEFAULT NULL,
  `updatedate` date DEFAULT NULL,
  `imageurl` text,
  PRIMARY KEY (`contentid`),
  UNIQUE KEY `contentid` (`contentid`),
  UNIQUE KEY `content_relation_unique` (`objecturi`,`relationtype`,`relatedto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `nomatches`
--

DROP TABLE IF EXISTS `nomatches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `nomatches` (
  `RaaId` varchar(65) NOT NULL,
  PRIMARY KEY (`RaaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `notcorrect`
--

DROP TABLE IF EXISTS `notcorrect`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `notcorrect` (
  `RaaId` varchar(100) NOT NULL,
  `LibrisId` varchar(100) NOT NULL,
  `NegativeVotes` int(10) unsigned DEFAULT '0',
  PRIMARY KEY (`RaaId`,`LibrisId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reportedfaults`
--

DROP TABLE IF EXISTS `reportedfaults`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reportedfaults` (
  `RAAId` varchar(100) NOT NULL,
  `LibrisId` varchar(100) NOT NULL,
  `ReportedTimes` int(11) DEFAULT NULL,
  PRIMARY KEY (`RAAId`,`LibrisId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `votecount`
--

DROP TABLE IF EXISTS `votecount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `votecount` (
  `RAAId` varchar(100) NOT NULL,
  `LibrisId` varchar(100) NOT NULL,
  `PositiveVotes` int(11) DEFAULT '0',
  `NegativeVotes` int(11) DEFAULT '0',
  PRIMARY KEY (`RAAId`,`LibrisId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='	';
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-05-23 16:53:13
