
-- phpMyAdmin SQL Dump
-- version 2.9.2-Debian-1.one.com1
-- http://www.phpmyadmin.net
-- 
-- Host: MySQL Server
-- Generation Time: Feb 02, 2010 at 01:21 PM
-- Server version: 5.0.32
-- PHP Version: 5.2.0-8+etch16
-- 
-- Database: `scherbaknn_com`
-- 

-- --------------------------------------------------------

-- 
-- Table structure for table `sch_applications`
-- 

DROP TABLE IF EXISTS `sch_applications`;
CREATE TABLE IF NOT EXISTS `sch_applications` (
  `application_id` int(11) NOT NULL auto_increment,
  `user_id` int(11) NOT NULL,
  `comment` text collate utf8_unicode_ci NOT NULL,
  `submited_on` timestamp NOT NULL default CURRENT_TIMESTAMP,
  `resolution` enum('pending','denied','granted') collate utf8_unicode_ci NOT NULL default 'pending',
  `resolved_on` timestamp NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`application_id`),
  KEY `FK_USERS_USER_ID` (`user_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

-- 
-- Table structure for table `sch_articles`
-- 

DROP TABLE IF EXISTS `sch_articles`;
CREATE TABLE IF NOT EXISTS `sch_articles` (
  `article_id` int(11) NOT NULL auto_increment,
  `lang` char(2) collate utf8_unicode_ci NOT NULL,
  `title` varchar(50) collate utf8_unicode_ci NOT NULL,
  `body` text collate utf8_unicode_ci NOT NULL,
  `created_on` timestamp NOT NULL default CURRENT_TIMESTAMP,
  PRIMARY KEY  (`article_id`,`lang`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

-- 
-- Table structure for table `sch_message`
-- 

DROP TABLE IF EXISTS `sch_message`;
CREATE TABLE IF NOT EXISTS `sch_message` (
  `message_id` int(11) NOT NULL auto_increment,
  `lang` char(2) collate utf8_unicode_ci NOT NULL,
  `code` varchar(20) collate utf8_unicode_ci NOT NULL,
  `message` varchar(200) collate utf8_unicode_ci NOT NULL,
  PRIMARY KEY  (`message_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Holds localized messages';

-- --------------------------------------------------------

-- 
-- Table structure for table `sch_users`
-- 

DROP TABLE IF EXISTS `sch_users`;
CREATE TABLE IF NOT EXISTS `sch_users` (
  `user_id` int(11) NOT NULL auto_increment,
  `email` varchar(255) collate utf8_unicode_ci NOT NULL,
  `fname` varchar(50) collate utf8_unicode_ci NOT NULL,
  `lname` varchar(50) collate utf8_unicode_ci NOT NULL,
  `hash` char(40) collate utf8_unicode_ci NOT NULL,
  `salt` int(6) NOT NULL,
  `registered_on` timestamp NOT NULL default CURRENT_TIMESTAMP,
  `last_logged_on` timestamp NOT NULL default '0000-00-00 00:00:00',
  PRIMARY KEY  (`user_id`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
