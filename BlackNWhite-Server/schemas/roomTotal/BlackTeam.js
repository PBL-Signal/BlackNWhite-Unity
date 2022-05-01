const mongoose = require('mongoose');
const { Schema } = mongoose;
const BlackUsers = require('./BlackUsers').schema;

const BlackTeam = new Schema({
    total_pita   : { type : Number, required : true },
    users   : { type : BlackUsers, required : true },
})

module.exports = mongoose.model('BlackTeam', BlackTeam);