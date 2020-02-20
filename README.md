# ACMGameStudioFPS

This project is a networked FPS developed by students at ACM Game Studio at UCLA.

//TODO Add more detail and pics

# Contributors:  

| Contributors | Branch Name |
| ------------- | ------------- |
| Kevin Chuang  | kevin  |
| Daniel Park  | danielp |
| Ziyan Wang (Jason)  | jason  |
| Nicholas Browning  | nick  |
| Bryant Ta  | bryant  |
| Liam O'Connor  | liam  |
| Alexander Ma  | alex  |
| Valentin Lagunes  | valentin  |
| Daniel Jaffe | danielj  |


# How to use branches:  

# Setup
First, make sure you're updated:  
`git pull`  
  
If creating a new branch  
`git branch <name_of_new_branch>`  
  
To do stuff in your new branch   
`git checkout <name_of_new_branch>` 
  
To check the branches (and find your branch)  
`git branch -a`
  
Add to list of remotes
`git remote add <easy_name_to_access> <name_of_branch>`
  
Now you do stuff in your scene and you want to push to your branch

`git add .`  
`git commit -m "commit message"`
`git pull`  
`git push origin <easy_name_to_access>`  

You're done with your new changes and you want to add it to the main branch  

`git checkout master`  
`git pull origin master`  
`git merge <name_of_branch>`  
`git push origin master`  
`git checkout <name_of_branch>`  

If at any point you need to update from the main branch  
`git pull origin master`  

If you want you can also issue a pull request (This is what most companies do I believe)

