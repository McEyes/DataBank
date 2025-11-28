export interface NavItem {
    /**
     * Nav Item Name
     */
    name: string;
    /**
     * Nav Item Icon
     */
    icon?: string;
    /**
     * Defines if nav menu item is a heading
     */
    isHeading?: boolean;
    /**
     * Defines if nav menu item contains a sub-menu
     */
    children?: NavItem[];
    /**
     * Defines a route the menu item should navigate to
     */
    href?: string;
    /**
     * Defines if the menu item is active
     */
    isActive?: boolean;
}
