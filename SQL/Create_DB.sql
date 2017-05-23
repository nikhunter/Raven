use raven;

DROP TABLE if exists raven.trips;
DROP TABLE if exists raven.logins;

CREATE TABLE IF NOT EXISTS `raven`.`logins` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `reg` VARCHAR(10) NOT NULL,
  `username` VARCHAR(32) NOT NULL,
  `password` mediumtext NOT NULL,
  PRIMARY KEY (`id`, `username`),
  UNIQUE INDEX `reg_UNIQUE` (`reg` ASC),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC))
COMMENT = 'Login table for Raven-GPS authentication';

insert into logins (`reg`,`username`,`password`) values (
    "BK79499",
    "RWejlgaard",
    "9151440965cf9c5e07f81eee6241c042a7b78e9bb2dd4f928a8f6da5e369cdffdd2b70c70663ee30d02115731d35f1ece5aad9b362aaa9850efa99e3d197212a  -"
);

insert into logins (`reg`,`username`,`password`) values (
    "BE70846",
    "NWMicheelsen",
    "9151440965cf9c5e07f81eee6241c042a7b78e9bb2dd4f928a8f6da5e369cdffdd2b70c70663ee30d02115731d35f1ece5aad9b362aaa9850efa99e3d197212a  -"
);

CREATE TABLE `raven`.`trips` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `time_started` DATETIME NOT NULL,
  `time_ended` DATETIME NOT NULL,
  `driver_reg` VARCHAR(20) NOT NULL,
  `log_file` MEDIUMBLOB NOT NULL,
  `driver_username` VARCHAR(32) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `reg_idx` (`driver_reg` ASC),
  INDEX `driver_username_idx` (`driver_username` ASC),
  CONSTRAINT `driver_reg_FK`
    FOREIGN KEY (`driver_reg`)
    REFERENCES `raven`.`logins` (`reg`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `driver_username_FK`
    FOREIGN KEY (`driver_username`)
    REFERENCES `raven`.`logins` (`username`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
COMMENT = 'Trip table for Raven-GPS';

insert into trips (`time_started`,`time_ended`,`driver_reg`,`log_file`,`driver_username`) values (
	STR_TO_DATE('05-18-2017 12:00:00','%m-%d-%Y %H:%i:%s'),
    STR_TO_DATE('05-18-2017 12:38:00','%m-%d-%Y %H:%i:%s'),
    "BK79499",
    "None",
    "RWejlgaard"
);

insert into trips (`time_started`,`time_ended`,`driver_reg`,`log_file`,`driver_username`) values (
	STR_TO_DATE('05-22-2017 14:00:00','%m-%d-%Y %H:%i:%s'),
    STR_TO_DATE('05-22-2017 14:43:25','%m-%d-%Y %H:%i:%s'),
    "BE70846",
    "None",
    "NWMicheelsen"
);

select * from logins;
SELECT * FROM trips
