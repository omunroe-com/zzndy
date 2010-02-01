-- phpMyAdmin SQL Dump
-- version 3.1.3.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 17, 2010 at 10:14 PM
-- Server version: 5.1.33
-- PHP Version: 5.2.9

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

--
-- Database: `scherbak`
--

-- --------------------------------------------------------

--
-- Table structure for table `sch_applications`
--

CREATE TABLE IF NOT EXISTS `sch_applications` (
  `application_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `comment` text COLLATE utf8_unicode_ci NOT NULL,
  `submited_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `resolution` enum('pending','denied','granted') COLLATE utf8_unicode_ci NOT NULL DEFAULT 'pending',
  `resolved_on` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`application_id`),
  KEY `FK_USERS_USER_ID` (`user_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=14 ;

-- --------------------------------------------------------

--
-- Table structure for table `sch_articles`
--

CREATE TABLE IF NOT EXISTS `sch_articles` (
  `article_id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(50) CHARACTER SET latin1 NOT NULL,
  `body` text CHARACTER SET latin1 NOT NULL,
  `created_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`article_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `sch_lang`
--

CREATE TABLE IF NOT EXISTS `sch_lang` (
  `message_id` int(11) NOT NULL AUTO_INCREMENT,
  `lang` char(2) CHARACTER SET latin1 NOT NULL,
  `code` varchar(20) CHARACTER SET latin1 NOT NULL,
  `message` varchar(200) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`message_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `sch_users`
--

CREATE TABLE IF NOT EXISTS `sch_users` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) CHARACTER SET latin1 NOT NULL,
  `fname` varchar(50) CHARACTER SET latin1 NOT NULL,
  `lname` varchar(50) CHARACTER SET latin1 NOT NULL,
  `hash` char(40) CHARACTER SET latin1 NOT NULL,
  `salt` int(6) NOT NULL,
  `registered_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_logged_on` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`user_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci AUTO_INCREMENT=37 ;
