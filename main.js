// import file system module
const fs = require('fs');
const { listenerCount } = require('process');

// function to parse an expression in the Egg language
function parseExpression(program) {
  program = skipSpace(program);
  let match, expr;
  // exec is not the same like in python
  // check if the program is a string between double quotes
  if (match = /^"([^"]*)"/.exec(program)) {
    // create a value node with the matched string as the value
    expr = {type: "value", value: match[1]};
  // check if the program is a number
  } else if (match = /^\d+\b/.exec(program)) {
    // create a value node with the matched number as the value
    expr = {type: "value", value: Number(match[0])};
    // check if the programm  is a list between brackets
  // check if the program is a word, which is any sequence of non-space, non-parenthesis characters
  } else if (match = /^[^\s(),#"]+/.exec(program)) {
    // create a word node with the matched characters as the name
    expr = {type: "word", name: match[0]};
  // if none of the above matches, throw a syntax error
  } else {
    throw new SyntaxError("Unexpected syntax: " + program);
  }

  // recursively call parseApply to parse the rest of the program and apply the operators
  return parseApply(expr, program.slice(match[0].length));
}

// function to skip whitespace characters in a string
function skipSpace(string) {
  // find the index of the first non-whitespace character
  let first = string.search(/\S/);
  // if there are no non-whitespace characters, return an empty string
  if (first == -1) return "";
  // return the string starting from the first non-whitespace character
  return string.slice(first);
}

// function to parse an expression that might involve function application
function parseApply(expr, program) {
  // skip any whitespace characters in the beginning of the program
  program = skipSpace(program);
  // if the program does not start with an opening parenthesis, return the expression and the rest of the program
  if (program[0] != "(") {
    return {expr: expr, rest: program};
  }

  // skip any whitespace characters after the opening parenthesis
  program = skipSpace(program.slice(1));
  // create an apply node with the previous expression as the operator and an empty array for the arguments
  expr = {type: "apply", operator: expr, args: []};
  // loop through the arguments until a closing parenthesis is found
  while (program[0] != ")") {
    // parse the next argument and add its expression to the arguments array
    let arg = parseExpression(program);
    expr.args.push(arg.expr);
    // skip any whitespace characters after the argument expression
    program = skipSpace(arg.rest);
    // if the next character is a comma, skip it and continue with the next argument
    if (program[0] == ",") {
      program = skipSpace(program.slice(1));
    // if the next character is not a comma or a closing parenthesis, throw a syntax error
    } else if (program[0] != ")") {
      throw new SyntaxError("Expected ',' or ')'");
    }
  }
  // recursively call parseApply to parse any subsequent function application
  return parseApply(expr, program.slice(1));
}


function parse(program) {
  let {expr, rest} = parseExpression(program);
  if (skipSpace(rest).length > 0) {
    throw new SyntaxError("Unexpected text after program");
  }
  return expr;
}

// Parses a program string into a data structure that represents the syntax tree.
function parse(program) {
    let {expr, rest} = parseExpression(program);
    if (skipSpace(rest).length > 0) {
      throw new SyntaxError("Unexpected text after program");
    }
    return expr;
  }
  
  // This object will hold the special forms that the interpreter knows about.
  const specialForms = Object.create(null);
  
  // Evaluates an expression in the context of a scope object.
  function evaluate(expr, scope) {
    // If the expression is a simple value, return the value.
    if (expr.type == "value") {
      return expr.value;
    }
    // If the expression is a variable, look up the variable in the scope and return its value.
    else if (expr.type == "word") {
      if (expr.name in scope) {
        return scope[expr.name];
      } else {
        throw new ReferenceError(`Undefined binding: ${expr.name}`);
      }
    }
    // If the expression is an application of a function, evaluate the operator and the operands and apply the function.
    else if (expr.type == "apply") {
      let {operator, args} = expr;
      if (operator.type == "word" && operator.name in specialForms) {
        // If the operator is a special form, call the function for the special form.
        return specialForms[operator.name](expr.args, scope);
      } else {
        // If the operator is a user-defined function, evaluate the function and pass the evaluated arguments to the function.
        let op = evaluate(operator, scope);
        if (typeof op == "function") {
          return op(...args.map(arg => evaluate(arg, scope)));
        } else {
          throw new TypeError("Applying a non-function.");
        }
      }
    }
  }

specialForms.if = (args, scope) => {
    if (args.length > 3){
        throw new ReferenceError("You are using to much arguments to if statement!")
    }
    // console.log("ARGS:");
    // console.log(args);
    // console.log("SCOPE:");
    // console.log(scope);
    // console.log("Len ARGS:");
    // console.log(args.length);
    // console.log("LEN SCOPE");
    // console.log(scope.length);
    // if args == 2 its pure if. if args == 3 its if with else
    if (evaluate(args[0], scope) !== false) {
        while (evaluate(args[0], scope) !== false) {
            return evaluate(args[1], scope);
          }
    } else {
        if (args.length == 3) {
            while (evaluate(args[0], scope) == false) {
                return evaluate(args[2], scope);
              }   
        }
    }
 
  }

  specialForms.ifold = (args, scope) => {
    if (args.length != 3) {
      throw new SyntaxError("Wrong number of args to if");
    } else if (evaluate(args[0], scope) !== false) {
      return evaluate(args[1], scope);
    } else {
      return evaluate(args[2], scope);
    }
  };

  specialForms.while = (args, scope) => {
    if (args.length != 2) {
      throw new SyntaxError("Wrong number of args to while");
    }
    while (evaluate(args[0], scope) !== false) {
      evaluate(args[1], scope);
    }
  
    // Since undefined does not exist in Egg, we return false,
    // for lack of a meaningful result.
    return false;
  };

  specialForms.do = (args, scope) => {
    let value = false;
    for (let arg of args) {
      value = evaluate(arg, scope);
    }
    return value;
  };

  specialForms.define = (args, scope) => {
    if (args.length != 2 || args[0].type != "word") {
      throw new SyntaxError("Incorrect use of define");
    }
    let value = evaluate(args[1], scope);
    scope[args[0].name] = value;
    return value;
  };

  specialForms.index = (args, scope) => {
    if (args.length !== 2) {
      throw new SyntaxError("Wrong number of arguments to index");
    }
  
    const str = evaluate(args[0], scope);
    const substr = evaluate(args[1], scope);
  
    if (typeof str !== "string" || typeof substr !== "string") {
      throw new TypeError("Both arguments to index must be strings");
    }
  
    const index = str.indexOf(substr);
    return index;
  };
  
  specialForms.fun = (args, scope) => {
    if (!args.length) {
      throw new SyntaxError("Functions need a body");
    }
    let body = args[args.length - 1];
    let params = args.slice(0, args.length - 1).map(expr => {
      if (expr.type != "word") {
        throw new SyntaxError("Parameter names must be words");
      }
      return expr.name;
    });
  
    return function() {
      if (arguments.length != params.length) {
        throw new TypeError("Wrong number of arguments");
      }
      let localScope = Object.create(scope);
      for (let i = 0; i < arguments.length; i++) {
        localScope[params[i]] = arguments[i];
      }
      return evaluate(body, localScope);
    };
  };

  specialForms.charAt = (args, scope) => {
    if (args.length !== 2) {
      throw new SyntaxError("Wrong number of arguments to charAt");
    }
  
    const str = evaluate(args[0], scope);
    const pos = evaluate(args[1], scope);
  
    if (typeof str !== "string" || typeof pos !== "number" || pos < 0 || pos >= str.length) {
      throw new TypeError("Invalid arguments to charAt");
    }
  
    return str.charAt(pos);
  };

  specialForms.replace = (args, scope) => {
    if (args.length !== 3){
        throw new SyntaxError("Wrong number of arguments to replace");
    }

    let object = evaluate(args[0], scope);
    const index = evaluate(args[1], scope);
    const new_thing = evaluate(args[2], scope);
    if (Array.isArray(object)){
        object[index] = new_thing;
        return object;        
    } else{
        let a = object.split("");
        object[index] = new_thing;
        return a.join("");
    }
  }

  const topScope = Object.create(null);
  
  for (let op of ["+", "-", "*", "/", "==", "<", ">", "!="]) {
    topScope[op] = Function("a, b", `return a ${op} b;`);
  }

  topScope.out = value => {
    console.log(value);
    return value;
  };
  topScope["array"] = function() {
    return Array.prototype.slice.call(arguments, 0);
   }

    // Element of array
    topScope["element"] = function(array, i) {
        return array[i];
    }
    // len of string
    topScope["len"] = function(object){
        return object.length;
    }


  function run(program) {
    return evaluate(parse(program), Object.create(topScope));
  }
// read code into string 
var filename = "ExampleCodes/example_code12.os";
fs.readFile(filename, (err, inputD) => {
    if (err) throw err;
         run(inputD.toString());
 })
