  Updates by maxim@storagecraft.com

1) git-receive-pack handler now sets USER environment variable to user logged on to client's browser 
    (for Git hooks to work, authorization-related and maybe others)
2) now shows the logged-on user name everywhere
3) now fails git-receive-pack if the logged-on user is in the "limited readers" group
