CREATE TABLE IF NOT EXISTS
    RoleTypes (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        RoleName VARCHAR(128) NOT NULL UNIQUE
    );

    CREATE TABLE IF NOT EXISTS
    UserHashes (
        UserName VARCHAR(128) NOT NULL UNIQUE, --- Changed from ForeignKey Users.UserId
        Hash VARCHAR(1024) NOT NULL UNIQUE
    );

CREATE TABLE IF NOT EXISTS
    Users (
        UsersId INTEGER PRIMARY KEY AUTOINCREMENT,
        UserName VARCHAR(128) NOT NULL UNIQUE,
        HashedPassword VARCHAR(1024) NOT NULL,
        RoleTypesId INTEGER NOT NULL,
        FOREIGN KEY (UserName) REFERENCES UserHashes (UserName) --- Added
        FOREIGN KEY (RoleTypesId) REFERENCES RoleTypes (Id)
    );



CREATE TABLE IF NOT EXISTS
    UserBalances (
        UsersId INTEGER NOT NULL UNIQUE, 
        BalancePence INTEGER NOT NULL,
        FOREIGN KEY (UsersId) REFERENCES Users (UsersId)
    );

CREATE TABLE IF NOT EXISTS
    MessageStates (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        StateName VARCHAR(128) NOT NULL UNIQUE
    );

CREATE TABLE IF NOT EXISTS
    Messages (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        UsersId INTEGER NOT NULL UNIQUE,
        MessageStatesId INTEGER NOT NULL,
        DateTime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        FOREIGN KEY (UsersId) REFERENCES Users (UsersId),
        FOREIGN KEY (MessageStatesId) REFERENCES MessageStates (Id)
    );