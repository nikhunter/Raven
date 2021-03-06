USE raven;

DROP TABLE if exists raven.trips;
DROP TABLE if exists raven.logins;

CREATE TABLE IF NOT EXISTS `raven`.`logins` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `reg` VARCHAR(10) NOT NULL,
  `username` VARCHAR(32) NOT NULL,
  `password` mediumtext NOT NULL,
  PRIMARY KEY (`id`, `username`),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
COMMENT = 'Login table for Raven-GPS authentication';

INSERT INTO logins (`reg`,`username`,`password`) VALUES (
    "BK79499",
    "rwejlgaard",
    "B109F3BBBC244EB82441917ED06D618B9008DD09B3BEFD1B5E07394C706A8BB980B1D7785E5976EC049B46DF5F1326AF5A2EA6D103FD07C95385FFAB0CACBC86"
);

INSERT INTO logins (`reg`,`username`,`password`) VALUES (
    "BE70846",
    "nwmicheelsen",
    "B109F3BBBC244EB82441917ED06D618B9008DD09B3BEFD1B5E07394C706A8BB980B1D7785E5976EC049B46DF5F1326AF5A2EA6D103FD07C95385FFAB0CACBC86"
);

CREATE TABLE `raven`.`trips` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `time_started` DATETIME NOT NULL,
  `time_ended` DATETIME NOT NULL,
  `driver_username` VARCHAR(32) NOT NULL,
  `driver_reg` VARCHAR(20) NOT NULL,
  `log_file` LONGTEXT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `reg_idx` (`driver_reg` ASC),
  INDEX `driver_username_idx` (`driver_username` ASC),
  CONSTRAINT `driver_username_FK`
    FOREIGN KEY (`driver_username`)
    REFERENCES `raven`.`logins` (`username`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
COMMENT = 'Trip table for Raven-GPS';